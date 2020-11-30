using Common.Notifications;
using Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Persistence;
using Microsoft.EntityFrameworkCore;

namespace SearchApp.SignalR
{
    public class NotificationHub : Hub
    {
        private readonly IConnectionManager _manager;
        private readonly IMediator _mediator;
        private readonly DataContext _dataContext;

        public NotificationHub(IConnectionManager manager, IMediator mediator, DataContext dataContext)
        {
            _manager = manager;
            _mediator = mediator;
            _dataContext = dataContext;
        }
        private string GetUsername()
        {
            return Context.User?.Claims?
                .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }
        public  string GetConnectionId()
        {
            var username = GetUsername();
            _manager.AddConnection(username, Context.ConnectionId);

            return Context.ConnectionId;
        }
        public async Task ReadNotification(string notificationId)
        {
            var aircraft = await _dataContext.Notifications.Where((n) => n.Id == Guid.Parse(notificationId)).FirstOrDefaultAsync();
            aircraft.IsRead = true;
            await _dataContext.SaveChangesAsync();
        }
    }
}
