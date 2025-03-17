using Survey.Domain.Interfaces.Models;

namespace Survey.Domain.Events.Dispatcher
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(IDomainEvent domainEvent);
    }
}
