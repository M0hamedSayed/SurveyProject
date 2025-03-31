using Survey.Domain.Models.Identity;

namespace Survey.Application.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<ApplicationUser> GenerateUserTokensAsync(ApplicationUser user);
        public Task<string> RefreshTokenAsync();
        public Task<bool> Logout();
    }
}
