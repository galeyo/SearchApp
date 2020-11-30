using Application.Aircrafts.Dto;
using Application.Errors;
using AutoMapper;
using Common.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Aircrafts
{
    public class Edit
    {
        public class Command : IRequest<AircraftDto>
        {
            public int Id { get; set; }
            public string AircraftName { get; set; }
            public Int16 YearInService { get; set; }
            public string Description { get; set; }
            public string[] Categories { get; set; }
            public string[] Types { get; set; }
            public string Country { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.AircraftName).NotEmpty();
                RuleFor(x => x.Description).NotEmpty();
                RuleFor(x => x.YearInService).NotEmpty();
                RuleFor(x => x.Categories).NotEmpty();
                RuleFor(x => x.Types).NotEmpty();
                RuleFor(x => x.Country).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, AircraftDto>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly Nest.ElasticClient _elasticClient;
            private readonly IConfiguration _configuration;
            private readonly IUserAccessor _userAccessor;
            private readonly IHubNotificationHelper _hubNotification;

            public Handler(DataContext context, IMapper mapper, Nest.ElasticClient elasticClient, IConfiguration configuration, IUserAccessor userAccessor, IHubNotificationHelper hubNotification)
            {
                _context = context;
                _mapper = mapper;
                _elasticClient = elasticClient;
                _configuration = configuration;
                _userAccessor = userAccessor;
                _hubNotification = hubNotification;
            }
            public async Task<AircraftDto> Handle(Command request, CancellationToken cancellationToken)
            {
                var aircraft = await _context.Aircraft.FindAsync(new object[] { request.Id }, cancellationToken);
                if (aircraft == null)
                    throw new RestException(HttpStatusCode.NotFound, new { aircraft = "Not found" });

                aircraft.AircraftName = request.AircraftName ?? aircraft.AircraftName;
                aircraft.Description = request.Description ?? aircraft.Description;
                aircraft.Country = request.Country ?? aircraft.Country;
                aircraft.YearInService = request.YearInService;

                var aicraftCategories = await _context.Category.Where(x => x.AircraftCategories.Any(c => c.AircraftId == aircraft.Id)).ToListAsync();
                var aircraftTypes = await _context.Type.Where(x => x.AircraftTypes.Any(t => t.AircraftId == aircraft.Id)).ToListAsync();

                foreach (var category in request.Categories)
                {
                    if (!aicraftCategories.Any(c => c.CategoryName == category))
                    {
                        if (!_context.Category.Any(c => c.CategoryName == category))
                        {
                            var newCategoryId = _context.Category.Max(x => x.Id) + 1;
                            await _context.Category.AddAsync(new Domain.Category
                            {
                                Id = newCategoryId,
                                CategoryName = category
                            });
                            await _context.AircraftCategory.AddAsync(new Domain.AircraftCategory
                            {
                                AircraftId = aircraft.Id,
                                CategoryId = newCategoryId
                            });
                        } else
                        {
                            var categoryId = await _context.Category.Where(c => c.CategoryName == category).FirstOrDefaultAsync();
                            await _context.AircraftCategory.AddAsync(new Domain.AircraftCategory
                            {
                                AircraftId = aircraft.Id,
                                CategoryId = categoryId.Id
                            });
                        }

                    }
                }
                if (request.Categories.Length < aicraftCategories.Count)
                {
                    var isNotExistInReq = aicraftCategories.Where(ac => request.Categories.All(c => ac.CategoryName != c));
                    List<Domain.AircraftCategory> removeAircrafts = new List<Domain.AircraftCategory>();
                    foreach (var category in isNotExistInReq)
                    {
                        removeAircrafts.Add(new Domain.AircraftCategory
                        {
                            AircraftId = aircraft.Id,
                            CategoryId = category.Id
                        });
                    }
                    _context.AircraftCategory.RemoveRange(removeAircrafts);
                }
                foreach (var type in request.Types)
                {
                    if (!aircraftTypes.Any(c => c.TypeName == type))
                    {
                        if (!_context.Type.Any(c => c.TypeName == type))
                        {
                            var newTypeId = _context.Type.Max(x => x.Id) + 1;
                            await _context.Type.AddAsync(new Domain.Type
                            {
                                Id = newTypeId,
                                TypeName = type
                            });
                            await _context.AircraftType.AddAsync(new Domain.AircraftType
                            {
                                AircraftId = aircraft.Id,
                                TypeId = newTypeId
                            });
                        }
                        else
                        {
                            var typeId = await _context.Type.Where(c => c.TypeName == type).FirstOrDefaultAsync();
                            await _context.AircraftCategory.AddAsync(new Domain.AircraftCategory
                            {
                                AircraftId = aircraft.Id,
                                CategoryId = typeId.Id
                            });
                        }
                    }
                }
                if (request.Types.Length < aircraftTypes.Count)
                {
                    var isNotExistInReq = aircraftTypes.Where(ac => request.Types.All(c => ac.TypeName != c));
                    List<Domain.AircraftType> removeAircrafts = new List<Domain.AircraftType>();
                    foreach (var type in isNotExistInReq)
                    {
                        removeAircrafts.Add(new Domain.AircraftType
                        {
                            AircraftId = aircraft.Id,
                            TypeId = type.Id
                        });
                    }
                    _context.AircraftType.RemoveRange(removeAircrafts);
                }
                await CreateNotification(_userAccessor.GetCurrentUsername(), aircraft);
                var success = await _context.SaveChangesAsync();

                if (success > 0)
                {
                    var retAircraft = await _context.Aircraft.FindAsync(new object[] { aircraft.Id });
                    var aircraftDto = _mapper.Map<Domain.Aircraft, AircraftDto>(retAircraft);
                    var editDocInIndex = await _elasticClient.UpdateAsync(
                        new Nest.DocumentPath<AircraftDto>(aircraftDto.Id),
                        d => d.Index(_configuration["Elasticsearch:DefaultIndex"]).Doc(aircraftDto)
                    );
                    return aircraftDto;
                }

                if (success == 0)
                {
                    return _mapper.Map<Domain.Aircraft, AircraftDto>(aircraft);
                }
                throw new Exception("Problem saving changes");
            }

            private async Task CreateNotification(string username, Domain.Aircraft aircraft)
            {
                var subscribers = await _context.Users
                    .Where(x => x.Subscribes.Any(s => s.AircraftId == aircraft.Id))
                    .ToListAsync();
                var body = $"{username} has changed {aircraft.AircraftName}";
                foreach (var subscriber in subscribers)
                {
                    var dbNotification = await _context.Notifications.AddAsync(new Domain.Notification
                    {
                        UserId = subscriber.Id,
                        IsRead = false,
                        Body = body
                    });
                    var onlineUsers = _hubNotification.GetOnlineUsers();
                    foreach (var onlineUser in onlineUsers)
                    {
                        if (onlineUser == subscriber.UserName)
                        {
                            await _hubNotification.SendNotificationParallel(onlineUser, new Common.Notifications.NotificationDto
                            {
                                Body = dbNotification.Entity.Body,
                                Id = dbNotification.Entity.Id,
                                IsRead = dbNotification.Entity.IsRead
                            });
                        }
                    }
                }
                
            }
        }
    }
}
