using MediatR;
using Survey.Domain.Events.Dispatcher;
using Survey.Domain.Interfaces.Models;

namespace Survey.Infrastructure.Events
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IMediator _mediator;

        public DomainEventDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task DispatchAsync(IDomainEvent domainEvent)
        {
            await _mediator.Publish(domainEvent);
        }
    }
}
