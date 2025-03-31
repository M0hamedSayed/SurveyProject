using Shared.Enums;
using Survey.Domain.Interfaces.Models;

namespace Survey.Domain.Events
{
    public class ConfirmEmailEvent(string Email, Guid UserId, string Token) : IDomainEvent
    {
        public List<NotificationTypes> NotifyTypes => [NotificationTypes.Email];
    }
}
