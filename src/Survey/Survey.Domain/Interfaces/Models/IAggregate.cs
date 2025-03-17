namespace Survey.Domain.Interfaces.Models
{
    public interface IAggregate<T> : IAggregate, IEntity<T>
    {
    }
    public interface IAggregate: IEntity
    {
        IReadOnlyList<IDomainEvent> DomainEvents { get; }
        IDomainEvent[] ClearDomainEvents();
    }
    public interface IAggregateSoftDeletable<T> : IAggregate, IEntitySoftDeletable<T>
    {
    }
    public interface IAggregateSoftDeletable : IAggregate, IEntitySoftDeletable
    {
    }
}
