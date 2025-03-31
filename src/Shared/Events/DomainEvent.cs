namespace Shared.Events
{
    public abstract record DomainEvent
    {
        Guid EventId => Guid.NewGuid();
        public DateTime OccuredOn => DateTime.UtcNow;
        public string? EventType => GetType().AssemblyQualifiedName;
    }
}
