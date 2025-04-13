using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Notify.API.AppDbContext;
using Notify.API.Events.Realtime;
using Notify.API.Interfaces;
using Notify.API.Models;
using Shared.Events;

namespace Notify.API.Events.Integration
{
    public class SurveyActivatedConsumer : IConsumer<SurveyActivatedEvent>
    {
        private readonly IClientService _clientService;
        private readonly ILogger<SurveyActivatedConsumer> _logger;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly IHubContext<NotificationHub> _hubContext;
        public SurveyActivatedConsumer(ApplicationDbContext applicationDbContext ,ILogger<SurveyActivatedConsumer> logger, IClientService clientService, IHubContext<NotificationHub> hubContext, IUserConnectionManager userConnectionManager )
        {
            _logger = logger;
            _hubContext = hubContext;
            _applicationDbContext = applicationDbContext;
            _clientService = clientService;
            _userConnectionManager = userConnectionManager;
        }

        public async Task Consume(ConsumeContext<SurveyActivatedEvent> context)
        {
            var surveyId = context.Message.SurveyId;
            var surveyUrl = $"http://localhost:4200/survey/{surveyId}";

            int batchSize = 500;
            int currentPage = 1;
            bool hasMoreEmails = true;

            while (hasMoreEmails)
            {
                var batch = await _clientService.GetUsersBatchAsync(surveyId, currentPage, batchSize, true);
                if (batch?.Data?.List == null || !batch.Data.List.Any()) break;
                var emailEvents = new List<SendEmailEvent>();
                var notifications = new List<Notification>();

                foreach (var email in batch?.Data?.List ?? [])
                {
                    var messageId = Guid.NewGuid();
                    if(email is null) continue;
                    var title = "Survey Added";
                    var description = $"A new survey is Added for {context.Message.NameEn} | {context.Message.NameAr}";

                    if (context.Message.NotifyTypes.Contains(Shared.Enums.NotificationTypes.Email))
                    {
                        emailEvents.Add(new SendEmailEvent(
                        messageId,
                        email!,
                        $"{description}, Please follow the link to reply <a href='{surveyUrl}'>here</a>",
                        title,
                        true
                        ));
                    }
                    if (context.Message.NotifyTypes.Contains(Shared.Enums.NotificationTypes.AppNotification))
                        notifications.Add(new Notification
                        {
                            Id = Guid.NewGuid(),
                            UserEmail = email!,
                            Title = title,
                            Description = description,
                            TargetUrl = surveyId.ToString(),
                        });
                }
                if (notifications.Any())
                    _applicationDbContext.Notifications.AddRange(notifications);

                if (emailEvents.Any())
                {
                    await context.PublishBatch(emailEvents);
                }
                await _applicationDbContext.SaveChangesAsync();

                // Send real-time notifications
                var notificationTasks = notifications.Select(async notification =>
                {
                    if (_userConnectionManager.TryGetConnectionId(notification.UserEmail, out var connectionId))
                    {
                        await _hubContext.Clients.Client(connectionId)
                            .SendAsync("ReceiveNotification", notification);
                    }
                });
                await Task.WhenAll(notificationTasks);

                hasMoreEmails = batch?.Data?.hasNextPage ?? false;
                currentPage++;
            }

            _logger.LogInformation("Survey email notifications processed successfully.");
        }
    }
}
/*
 * Todo :
 * get all notification not read count
 * reaad all notification
 * get paginated notification
 * read one notification
 * delete survey
 * update end time survey
 * handle reminder survey
 * 
 */
