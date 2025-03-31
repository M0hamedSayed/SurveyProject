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
    internal class SurveyReminderService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SurveyReminderService(IServiceScopeFactory serviceScopeFactory, ILogger<SurveyReminderService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var reminderTime = DateTime.UtcNow.AddHours(4);
                var surveysToRemind = await unitOfWork.SurveyRepository.GetTableNoTracking()
                    .Where(s => s.EndDate < reminderTime && s.EndDate > DateTime.UtcNow && !s.ReminderSent && s.IsActive && s.IsRequired)
                    .ToListAsync();

                foreach (var survey in surveysToRemind)
                {
                    survey.SuveyReminder();
                    unitOfWork.SurveyRepository.Update(survey);
                }

                await unitOfWork.Complete();

                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
        }
    }
}
