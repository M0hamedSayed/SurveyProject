using Shared.Enums;
using Survey.Domain.Interfaces.Models;
using Survey.Domain.Models.Survey;

namespace Survey.Domain.Events
{
    public record SurveyCreatedEvent(Surveys survey) : IDomainEvent
    {
        public List<NotificationTypes> NotifyTypes => [NotificationTypes.AppNotification, NotificationTypes.Email];
    }
}
