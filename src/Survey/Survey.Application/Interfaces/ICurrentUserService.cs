using Survey.Domain.Models.Identity;

namespace Survey.Application.Interfaces
{
    public interface ICurrentUserService
    {
        public Task<ApplicationUser> GetUserAsync();
        public Guid GetUserId();
        public Task<(ApplicationUser? user, IList<string> roles)> GetCurrentUserWithRolesAsync();
    }
}
