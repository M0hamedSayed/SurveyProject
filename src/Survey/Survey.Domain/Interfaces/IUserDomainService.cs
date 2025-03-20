using Survey.Domain.Models.Identity;

namespace Survey.Domain.Interfaces
{
    public interface IUserDomainService
    {
        Task<ApplicationUser> RegisterUserAsync(string fullName, string email, string password, string role, Guid? managerId, bool emailConfirmed = false);
    }
}
