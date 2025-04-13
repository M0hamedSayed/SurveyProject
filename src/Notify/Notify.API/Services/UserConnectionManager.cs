using System.Collections.Concurrent;
using Notify.API.Interfaces;

namespace Notify.API.Services
{
    public class UserConnectionManager : IUserConnectionManager
    {
        private readonly ConcurrentDictionary<string, string> _userConnections = new();

        public void AddConnection(string email, string connectionId)
        {
            _userConnections[email] = connectionId;
        }

        public void RemoveConnection(string connectionId)
        {
            var item = _userConnections.FirstOrDefault(x => x.Value == connectionId);
            if (!string.IsNullOrEmpty(item.Key))
            {
                _userConnections.TryRemove(item.Key, out _);
            }
        }

        public bool TryGetConnectionId(string email, out string connectionId)
        {
            return _userConnections.TryGetValue(email, out connectionId!);
        }

        public IReadOnlyDictionary<string, string> GetAllConnections() => _userConnections;
    }
}
