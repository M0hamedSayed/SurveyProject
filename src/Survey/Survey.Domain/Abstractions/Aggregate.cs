using Shared.Events;
using Survey.Domain.Interfaces.Models;

namespace Survey.Domain.Abstractions
{
    public abstract class Aggregate<T> : Entity<T>, IAggregate<T>
    {
        private readonly List<DomainEvent> _domainEvents = new();
        public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents;

        public void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public DomainEvent[] ClearDomainEvents()
        {
            DomainEvent[] dequeuedEvents = _domainEvents.ToArray();
            _domainEvents.Clear();
            return dequeuedEvents;
        }
    }

    public abstract class AggregateSoftDeletable<T> : EntitySoftDeletable<T>, IAggregateSoftDeletable<T>
    {
        private readonly List<DomainEvent> _domainEvents = new();
        public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents;

        public void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public DomainEvent[] ClearDomainEvents()
        {
            DomainEvent[] dequeuedEvents = _domainEvents.ToArray();
            _domainEvents.Clear();
            return dequeuedEvents;
        }
    }
}
