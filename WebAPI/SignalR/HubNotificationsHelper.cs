using Common.Notifications;
using Common.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchApp.SignalR
{
    public class HubNotificationsHelper : IHubNotificationHelper
    {
        private readonly IHubContext<NotificationHub> _context;
        private readonly IConnectionManager _connection;

        public HubNotificationsHelper(IHubContext<NotificationHub> context, IConnectionManager connection)
        {
            _context = context;
            _connection = connection;
        }

        public IEnumerable<string> GetOnlineUsers()
        {
            return _connection.OnlineUsers;
        }

        public async Task<Task> SendNotificationParallel(string username, NotificationDto message)
        {
            var connections = _connection.GetConnections(username);

            try
            {
                if (connections != null & connections.Count > 0)
                {
                    foreach (var conn in connections)
                    {
                        try
                        {
                            await _context.Clients
                                .Clients(new List<string>(connections).AsReadOnly())
                                .SendAsync("ReceiveNotification", message);
                        }
                        catch (Exception ex)
                        {

                            throw ex;
                        }
                    }
                }
                return Task.CompletedTask;
            }
            catch (Exception)
            {

                throw new Exception("ERROR");
            }
        }

        public async void SendNotificationToAll(NotificationDto message)
        {
            await _context.Clients.All.SendAsync("ReceiveNotification", message);
        }
    }
}
