using Shared.Events;

namespace Survey.Domain.Events.Dispatcher
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync<T>(T domainEvent, CancellationToken cancellationToken) where T : class;
    }
}
