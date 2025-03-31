using Shared.Events;

namespace Survey.Domain.Interfaces.Models
{
    public interface IAggregate<T> : IAggregate, IEntity<T>
    {
    }
    public interface IAggregate : IEntity
    {
        IReadOnlyList<DomainEvent> DomainEvents { get; }
        DomainEvent[] ClearDomainEvents();
    }
    public interface IAggregateSoftDeletable<T> : IAggregate, IEntitySoftDeletable<T>
    {
    }
    public interface IAggregateSoftDeletable : IAggregate, IEntitySoftDeletable
    {
    }
}
