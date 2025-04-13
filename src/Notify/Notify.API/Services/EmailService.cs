using System.Net.Sockets;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Notify.API.Dtos;
using Notify.API.Interfaces;
using Polly;
using Shared.Helpers;

namespace Notify.API.Services
{
    public class EmailService : IEmailService,IDisposable
    {
        private readonly EmailSettings _emailSettings;
        private readonly SmtpClient _smtpClient = new ();
        private readonly AsyncPolicy _retryPolicy;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
            _retryPolicy = Policy
                .Handle<SocketException>()
                .Or<SmtpCommandException>()
                .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            // connect to email provider
            ConnectSmtpClient(_emailSettings);
        }

        private void ConnectSmtpClient(EmailSettings emailSettings)
        {
            _smtpClient.Connect(emailSettings.Host, emailSettings.Port, true);
            _smtpClient.Authenticate(emailSettings.FromEmail, emailSettings.Password);
        }

        public async Task SendEmailAsync(EmailMessage email)
        {
            var bodyBuilder = new BodyBuilder
            {
                TextBody = email.IsHtml ? null : email.Body,
                HtmlBody = email.IsHtml ? email.Body : null
            };

            var message = new MimeMessage
            {
                Body = bodyBuilder.ToMessageBody()
            };
            message.From.Add(new MailboxAddress("Survey Project", _emailSettings.FromEmail));
            // Add recipients
            foreach (var recipient in email.To)
                message.To.Add(MailboxAddress.Parse(recipient));
            // Add CC
            if (email.Cc is not null)
            {
                foreach (var cc in email.Cc)
                    message.Cc.Add(MailboxAddress.Parse(cc));
            }

            // Add BCC
            if (email.Bcc is not null)
            {
                foreach (var bcc in email.Bcc)
                    message.Bcc.Add(MailboxAddress.Parse(bcc));
            }
            // Add attachments if any
            if (email.Attachments is not null)
            {
                foreach (var attachment in email.Attachments)
                {
                    using var stream = attachment.OpenReadStream();
                    bodyBuilder.Attachments.Add(attachment.FileName, stream);
                }
            }
            message.Subject = email?.Subject;

            await _retryPolicy.ExecuteAsync(async () =>
            {
                if (!_smtpClient.IsConnected)
                    ConnectSmtpClient(_emailSettings);
                await _smtpClient.SendAsync(message);
            });
        }

        public void Dispose()
        {
            _smtpClient.Disconnect(true);
            _smtpClient.Dispose();
        }
    }
}
