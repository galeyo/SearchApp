using Common.Notifications;
using Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SearchApp.SignalR
{
    public class NotificationHub : Hub
    {
        private readonly IConnectionManager _manager;
        private readonly IMediator _mediator;

        public NotificationHub(IConnectionManager manager, IMediator mediator)
        {
            _manager = manager;
            _mediator = mediator;
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
    }
}
