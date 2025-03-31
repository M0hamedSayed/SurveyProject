using MassTransit;
using Shared.Events;
using Survey.Domain.Events.Dispatcher;

namespace Survey.Infrastructure.Events
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public DomainEventDispatcher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task DispatchAsync<T>(T domainEvent, CancellationToken cancellationToken) where T : class
        {
            await _publishEndpoint.Publish(domainEvent, cancellationToken);
        }
    }
}
