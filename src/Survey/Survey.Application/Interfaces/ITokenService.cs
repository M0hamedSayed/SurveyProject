using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Survey.Domain.Models.Identity;

namespace Survey.Application.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateGwtToken(ApplicationUser user);
        string GenerateRefreshToken();
        public ClaimsPrincipal? GetPrincipalFromJwtToken(string? token);
    }
}
