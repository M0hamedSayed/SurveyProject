using MediatR;
using Survey.Domain.Enums;

namespace Survey.Domain.Interfaces.Models
{
    public interface IDomainEvent : INotification
    {
        Guid EventId => Guid.NewGuid();
        public DateTime OccuredOn => DateTime.UtcNow;
        public string? EventName => GetType().AssemblyQualifiedName;
        public string EventType { get; set; }
        public List<NotificationTypes> NotifyTypes { get; set; }
    }
}
