using MediatR;
using Shared.Enums;

namespace Survey.Domain.Interfaces.Models
{
    public interface IDomainEvent : INotification
    {
        Guid EventId => Guid.NewGuid();
        public DateTime OccuredOn => DateTime.UtcNow;
        public string? EventName => GetType().AssemblyQualifiedName;
        public List<NotificationTypes> NotifyTypes { get; }
    }
}
