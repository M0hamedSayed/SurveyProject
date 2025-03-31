using Shared.Enums;

namespace Shared.Events
{
    public record SurveyReminderEvent(Guid SurveyId, string NameEn, string NameAr, Guid managerId) : DomainEvent
    {
        public List<NotificationTypes> NotifyTypes => [NotificationTypes.AppNotification, NotificationTypes.Email];
    }
}
