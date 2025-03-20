using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Survey.Domain.Interfaces.Repositories;
using Survey.Domain.Models.Identity;
using Survey.Infrastructure.DatabaseContext;

namespace Survey.Infrastructure.Repositories
{
    public class UserRepository : GenericrepositoryAsync<ApplicationUser>, IUserRepository
    {
        public UserManager<ApplicationUser> UserManager { get; }

        public SignInManager<ApplicationUser> SignInManager { get; }

        public RoleManager<ApplicationRole> RoleManager { get; }


        private readonly DbSet<ApplicationUser> _users;

        public UserRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager) : base(context)
        {
            _users = context.Set<ApplicationUser>();
            UserManager = userManager;
            RoleManager = roleManager;
            SignInManager = signInManager;
        }
    }
}
