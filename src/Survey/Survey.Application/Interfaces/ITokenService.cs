using System.Security.Claims;
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
