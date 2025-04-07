using MassTransit;
using Microsoft.EntityFrameworkCore;
using Notify.API.AppDbContext;
using Notify.API.Dtos;
using Notify.API.Interfaces;
using Notify.API.Models;
using Shared.Events;

namespace Notify.API.Events.Integration
{
    public class SendEmailConsumer : IConsumer<SendEmailEvent>
    {
        private readonly ILogger<SendEmailConsumer> _logger;
        private readonly IEmailService _emailService;
        private readonly ApplicationDbContext _context;

        public SendEmailConsumer(ILogger<SendEmailConsumer> logger, IEmailService emailService, ApplicationDbContext context)
        {
            _emailService = emailService;
            _context = context;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<SendEmailEvent> context)
        {
            // handle idempotency
            if (await _context.NotifyMessages.AnyAsync(n => n.Id == context.Message.Id))
            {
                _logger.LogWarning("Skipping duplicate email notification for {Email}", context.Message.Email);
                return;
            }
            var emailMsg = new EmailMessage
            {
                To = new List<string> { context.Message.Email },
                Subject = context.Message.Subject,
                Body = context.Message.Body,
                IsHtml = context.Message.IsHtml,
            };
            await _emailService.SendEmailAsync(emailMsg);
            // save message to idempotency
            var msg = new NotifyMessage
            {
                Id = context.Message.Id,
                NotificationType = Shared.Enums.NotificationTypes.Email
            };
            _context.NotifyMessages.Add(msg);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Email sent successfully");
        }
    }
}
