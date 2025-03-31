using Notify.API.Dtos;

namespace Notify.API.Interfaces
{
    public interface IEmailService
    {
        public Task SendEmailAsync(EmailMessage email);
    }
}
