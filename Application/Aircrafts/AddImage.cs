using Application.Aircrafts.Dto;
using Application.Errors;
using AutoMapper;
using Common.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Aircrafts
{
    public class AddImage
    {
        public class Command : IRequest
        {
            public IFormFile File { get; set; }
            public int AircraftId { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;
            private readonly IWebHostEnvironment _hostEnvironment;
            private readonly IMapper _mapper;
            private readonly Nest.ElasticClient _elasticClient;
            private readonly IConfiguration _configuration;
            private readonly IHubNotificationHelper _hubNotification;

            public Handler(DataContext context, IWebHostEnvironment hostEnvironment, IMapper mapper, Nest.ElasticClient elasticClient, IConfiguration configuration, IHubNotificationHelper hubNotification)
            {
                _context = context;
                _hostEnvironment = hostEnvironment;
                _mapper = mapper;
                _elasticClient = elasticClient;
                _configuration = configuration;
                _hubNotification = hubNotification;
            }
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                if (!await _context.Aircraft.Where(x => x.Id == request.AircraftId).AnyAsync())
                    throw new RestException(HttpStatusCode.NotFound, new { AircraftId = "Aircraft not found" });
                string uploads = Path.Combine(_hostEnvironment.WebRootPath, "api", "uploads");
                string filePath = Path.Combine(uploads, request.File.FileName);
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(fileStream);
                }
                var image = new Image
                {
                    AircraftId = request.AircraftId,
                    ImageUrl = "uploads/" + request.File.FileName
                };
                await _context.Images.AddAsync(image);

                var success = await _context.SaveChangesAsync() > 0;

                if (success)
                {
                    var retAircraft = await _context.Aircraft.FindAsync(new object[] { request.AircraftId }, cancellationToken);
                    var aircraftDto = _mapper.Map<Aircraft, AircraftDto>(retAircraft);
                    var editDocInIndex = await _elasticClient.UpdateAsync(
                        new Nest.DocumentPath<AircraftDto>(aircraftDto.Id),
                        d => d.Index(_configuration["Elasticsearch:DefaultIndex"]).Doc(aircraftDto)
                    );
                    return Unit.Value;
                }

                throw new Exception("Problem saving changes");
            }

            private async Task CreateNotification(string username, Aircraft aircraft)
            {
                var subscribers = await _context.Users
                    .Where(x => x.Subscribes.Any(s => s.AircraftId == aircraft.Id))
                    .ToListAsync();
                var body = $"{username} added new photo to {aircraft.AircraftName}";
                foreach (var subscriber in subscribers)
                {
                    await _context.Notifications.AddAsync(new Domain.Notification
                    {
                        UserId = subscriber.Id,
                        IsRead = false,
                        Body = body
                    });
                }
                var onlineUsers = _hubNotification.GetOnlineUsers();
                var usersToSendNotification = onlineUsers.Where(o => subscribers.Any(x => x.UserName == o));
                foreach (var users in usersToSendNotification)
                {
                    //await _hubNotification.SendNotificationParallel(users, body);
                }
            }
        }
    }
}
