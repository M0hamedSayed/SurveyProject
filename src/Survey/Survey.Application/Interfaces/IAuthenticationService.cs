using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Survey.Domain.Models.Identity;

namespace Survey.Application.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<ApplicationUser> GenerateUserTokensAsync(ApplicationUser user);
        public Task<string> RefreshTokenAsync();
        public  Task<bool> Logout();
    }
}
