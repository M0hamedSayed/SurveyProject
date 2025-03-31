using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Survey.Domain.Interfaces.Repositories;

namespace Survey.Application.Services.Background
{
    public class SurveyDeactivationService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SurveyDeactivationService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var now = DateTime.UtcNow;
                var surveysToDeactivate = await unitOfWork.SurveyRepository.GetTableNoTracking()
                    .Where(s => s.EndDate <= now && s.IsActive).ToListAsync();

                if (surveysToDeactivate.Any())
                {
                    foreach (var survey in surveysToDeactivate)
                    {
                        survey.Deactivate();
                        unitOfWork.SurveyRepository.Update(survey);
                    }

                    await unitOfWork.Complete();
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
