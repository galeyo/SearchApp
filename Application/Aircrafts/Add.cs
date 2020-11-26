using Application.Aircrafts.Dto;
using Application.Errors;
using Common.Notifications;
using AutoMapper;
using Common.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Aircrafts
{
    public class Add
    {
        public class Command : IRequest<AircraftDto>
        {
            public IFormFile File { get; set; }
            public string AircraftName { get; set; }
            public Int16 YearInService { get; set; }
            public string Description { get; set; }
            public string Categories { get; set; }
            public string Types { get; set; }
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
            private readonly IWebHostEnvironment _hostEnvironment;
            private readonly IMapper _mapper;
            private readonly Nest.ElasticClient _elasticClient;
            private readonly IUserAccessor _userAccessor;
            private readonly IHubNotificationHelper _hubNotification;

            public Handler(DataContext context, IWebHostEnvironment hostEnvironment, IMapper mapper, Nest.ElasticClient elasticClient, IUserAccessor userAccessor, IHubNotificationHelper hubNotification)
            {
                _context = context;
                _hostEnvironment = hostEnvironment;
                _mapper = mapper;
                _elasticClient = elasticClient;
                _userAccessor = userAccessor;
                _hubNotification = hubNotification;
            }
            public async Task<AircraftDto> Handle(Command request, CancellationToken cancellationToken)
            {
                if (await _context.Aircraft.Where(x => x.AircraftName == request.AircraftName).AnyAsync())
                    throw new RestException(HttpStatusCode.BadRequest, new { AircraftName = "Aircraft name already exist" });
                string uploads = Path.Combine(_hostEnvironment.WebRootPath, "api", "uploads");
                string filePath = Path.Combine(uploads, request.File.FileName);
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(fileStream);
                }
                var aircraftId = _context.Aircraft.Max(el => el.Id) + 1;
                var categoryId = _context.Category.Max(el => el.Id) + 1;
                var typeId = _context.Type.Max(el => el.Id) + 1;
                var categories = JsonSerializer.Deserialize<CategoryDto>(request.Categories, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                var types = JsonSerializer.Deserialize<TypeDto>(request.Types, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                foreach (var category in categories.Categories)
                {
                    if (!_context.Category.Any(c => c.CategoryName == category))
                    {
                        await _context.AircraftCategory.AddAsync(new AircraftCategory
                        {
                            AircraftId = aircraftId,
                            CategoryId = categoryId
                        });
                        await _context.Category.AddAsync(new Category
                        {
                            Id = categoryId,
                            CategoryName = category,
                        });
                        categoryId++;
                    }
                    else
                    {
                        var existingCategory = await _context.Category.Where(c => c.CategoryName == category).FirstOrDefaultAsync();
                        var existingCategoryId = existingCategory.Id;
                        await _context.AircraftCategory.AddAsync(new AircraftCategory
                        {
                            AircraftId = aircraftId,
                            CategoryId = existingCategoryId
                        });
                    }
                }

                foreach (var type in types.Types)
                {
                    if (!_context.Type.Any(c => c.TypeName == type))
                    {
                        await _context.AircraftType.AddAsync(new AircraftType
                        {
                            AircraftId = aircraftId,
                            TypeId = typeId
                        });
                        await _context.Type.AddAsync(new Domain.Type
                        {
                            Id = typeId,
                            TypeName = type,
                        });
                        typeId++;
                    }
                    else
                    {
                        var existingType = await _context.Type.Where(t => t.TypeName == type).FirstOrDefaultAsync();
                        var existingTypeId = existingType.Id;
                        await _context.AircraftType.AddAsync(new AircraftType
                        {
                            AircraftId = aircraftId,
                            TypeId = existingTypeId
                        });
                    }
                }
                var image = new Image
                {
                    AircraftId = aircraftId,
                    ImageUrl = "uploads/" + request.File.FileName
                };
                await _context.Images.AddAsync(image);
                var aircraft = new Aircraft
                {
                    Id = aircraftId,
                    AircraftName = request.AircraftName,
                    Description = request.Description,
                    YearInService = request.YearInService,
                    Country = request.Country,
                };
                await _context.Aircraft.AddAsync(aircraft);

                await CreateNotification(_userAccessor.GetCurrentUsername(), aircraft.AircraftName);
                var success = await _context.SaveChangesAsync() > 0;

                if (success)
                {
                    var retAircraft = await _context.Aircraft.FindAsync(new object[] { aircraftId }, cancellationToken);
                    var aircraftDto = _mapper.Map<Aircraft, AircraftDto>(retAircraft);
                    var addDocInIndexRes = await _elasticClient.IndexDocumentAsync(aircraftDto);
                    if (addDocInIndexRes.OriginalException != null) throw addDocInIndexRes.OriginalException;
                    return aircraftDto;
                }

                throw new Exception("Problem saving changes");
            }

            // Send all users notification
            private async Task CreateNotification(string username, string aircraftName)
            {
                var allUsers = await _context.Users.ToListAsync();
                var body = $"{username} added new aircraft {aircraftName}";
                var notifications = new List<NotificationDto>();
                foreach (var user in allUsers)
                {
                    var dbNotification = await _context.Notifications.AddAsync(new Notification
                    {
                        UserId = user.Id,
                        IsRead = false,
                        Body = body
                    });
                    var onlineUsers = _hubNotification.GetOnlineUsers();

                }
                //_hubNotification.SendNotificationToAll(body);
            }
        }
    }
}

