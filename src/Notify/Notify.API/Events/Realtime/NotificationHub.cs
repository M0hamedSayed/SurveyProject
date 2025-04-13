using System.Collections.Concurrent;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Notify.API.Interfaces;

namespace Notify.API.Events.Realtime
{
    [Authorize]
    public class NotificationHub(IUserConnectionManager userConnectionManager) : Hub
    {
        private readonly IUserConnectionManager _userConnections = userConnectionManager;

        public override async Task OnConnectedAsync()
        {
            var email = Context.User?.FindFirst(ClaimTypes.Email)?.Value
                       ?? Context.User?.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;

            if (!string.IsNullOrEmpty(email))
            {
                _userConnections.AddConnection(email,Context.ConnectionId);
                await Clients.Client(Context.ConnectionId).SendAsync("Connected", Context.ConnectionId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _userConnections.RemoveConnection(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendNotificationToUser(string email, object message)
        {
            if (_userConnections.TryGetConnectionId(email, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveNotification", message);
            }
        }
    }
}
