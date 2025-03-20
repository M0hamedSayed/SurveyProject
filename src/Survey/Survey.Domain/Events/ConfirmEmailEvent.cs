using Shared.Enums;
using Survey.Domain.Interfaces.Models;

namespace Survey.Domain.Events
{
    public class ConfirmEmailEvent(string Email, Guid UserId, string Token) : IDomainEvent
    {
        public EventTypes EventType => EventTypes.ConfirmUserEmail;
        public List<NotificationTypes> NotifyTypes =>  [ NotificationTypes.Email ];

    }
}
