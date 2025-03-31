using System.Threading;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Events;
using Survey.Domain.Events.Dispatcher;
using Survey.Domain.Interfaces.Repositories;

namespace Survey.Application.Services.Background
{
    public class SurveyActivationService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public SurveyActivationService(IServiceScopeFactory serviceScopeFactory) 
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var dispatcher = scope.ServiceProvider.GetRequiredService<IDomainEventDispatcher>();

                var now = DateTime.UtcNow;
                var surveysToActivate = await unitOfWork.SurveyRepository.GetTableNoTracking()
                    .Where(s => s.StartDate <= now && !s.IsActive && s.EndDate > now).ToListAsync();

                if (surveysToActivate.Any())
                {
                    foreach (var survey in surveysToActivate)
                    {
                        survey.Activate();
                        unitOfWork.SurveyRepository.Update(survey);
                    }

                    await unitOfWork.Complete();
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
