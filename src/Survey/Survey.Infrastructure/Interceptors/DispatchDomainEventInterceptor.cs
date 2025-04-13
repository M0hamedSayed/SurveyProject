using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Shared.Events;
using Survey.Domain.Events.Dispatcher;
using Survey.Domain.Interfaces.Models;

namespace Survey.Infrastructure.Interceptors
{
    public class DispatchDomainEventInterceptor : SaveChangesInterceptor
    {
        //private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly IPublishEndpoint _publishEndpoint;
        public DispatchDomainEventInterceptor(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }
        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken); ;
        }

        public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            await DispatchDomainEvents(eventData.Context, cancellationToken);

            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }
        private async Task DispatchDomainEvents(DbContext? context, CancellationToken cancellationToken)
        {
            if (context == null) return;

            // Create a new DI scope
            //using var scope = _scopeFactory.CreateScope();
            //var eventDispatcher = scope.ServiceProvider.GetRequiredService<IDomainEventDispatcher>();
            //var endpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();


            var aggregates = context.ChangeTracker
                .Entries<IAggregate>()
                .Where(a => a.Entity.DomainEvents != null && a.Entity.DomainEvents.Any())
                .Select(a => a.Entity)
                .ToList();  // Convert to List only once

            var domainEvents = aggregates
                .SelectMany(a => a.DomainEvents)
                .ToList();  // Extract domain events

            foreach (var aggregate in aggregates)
            {
                aggregate.ClearDomainEvents(); // Clear events from entities
            }

            foreach (var domainEvent in domainEvents)
            {
                if (domainEvent is SurveyActivatedEvent surveyActivatedEvent)
                    await _publishEndpoint.Publish(surveyActivatedEvent, cancellationToken);
                else if (domainEvent is SurveyReminderEvent reminderEvent)
                    await _publishEndpoint.Publish(reminderEvent, cancellationToken);
            }
        }
    }
}
