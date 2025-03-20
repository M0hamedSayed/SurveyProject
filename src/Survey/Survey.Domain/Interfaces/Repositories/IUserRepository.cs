using Microsoft.AspNetCore.Identity;
using Survey.Domain.Models.Identity;

namespace Survey.Domain.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepositoryAsync<ApplicationUser>
    {
        public UserManager<ApplicationUser> UserManager { get; }
        public SignInManager<ApplicationUser> SignInManager {  get; } 
        public RoleManager<ApplicationRole> RoleManager { get; }
    }
}
