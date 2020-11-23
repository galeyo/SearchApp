using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IHubNotificationHelper
    {
        void SendNotificationToAll(string message);
        IEnumerable<string> GetOnlineUsers();
        Task<Task> SendNotificationParallel(string username);
    }
}
