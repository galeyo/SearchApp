using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Interfaces
{
    public interface IConnectionManager
    {
        void AddConnection(string username, string connectionId);
        void RemoveConnection(string connectionId);
        HashSet<string> GetConnections(string username);
        IEnumerable<string> OnlineUsers { get; }
    }
}
