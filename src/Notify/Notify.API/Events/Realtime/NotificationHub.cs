using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Notify.API.Events.Realtime
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> _userConnections = new();

        public override async Task OnConnectedAsync()
        {
            var email = Context.User?.Claims?.SingleOrDefault(claim => claim.Type == "Email")?.Value;
            if (!string.IsNullOrEmpty(email))
            {
                _userConnections[email] = Context.ConnectionId;
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var email = _userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (!string.IsNullOrEmpty(email))
            {
                _userConnections.TryRemove(email, out _);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendNotificationToUser(string email, string message)
        {
            if (_userConnections.TryGetValue(email, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveNotification", message);
            }
        }
    }
}
