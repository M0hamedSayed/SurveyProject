using Shared.Enums;

namespace Shared.Events
{
    public record SurveyActivatedEvent(Guid SurveyId, string NameEn, string NameAr, Guid managerId) : DomainEvent
    {
        public List<NotificationTypes> NotifyTypes => [NotificationTypes.AppNotification, NotificationTypes.Email];
    }
}
