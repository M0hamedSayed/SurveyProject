using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Helpers;
using Survey.Application.Interfaces;
using Survey.Domain.Interfaces.Repositories;
using Survey.Domain.Models.Identity;

namespace Survey.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly TokenSettings _tokenSettings;
        private readonly IUnitOfWork _unitOfWork;

        public TokenService(IOptions<TokenSettings> tokenSettings, IUnitOfWork unitOfWork)
        {
            _tokenSettings = tokenSettings.Value;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> GenerateGwtToken(ApplicationUser user)
        {
            List<Claim> claims = await handleUserClaimsAsync(user);
            // security key
            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.Secret));
            // Create a SigningCredentials object with the security key and the HMACSHA256 algorithm.
            SigningCredentials signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
            _tokenSettings.Issuer,
            _tokenSettings.Audience,
            claims,
                expires: DateTime.UtcNow.AddHours(Convert.ToDouble(_tokenSettings.AccessTokenExpireDate)),
                signingCredentials: signingCredentials
            );
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return accessToken;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            var randomNumberGenerate = RandomNumberGenerator.Create();
            randomNumberGenerate.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal? GetPrincipalFromJwtToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidAudience = _tokenSettings.Audience,
                ValidateIssuer = true,
                ValidIssuer = _tokenSettings.Issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.Secret)),
                ValidateLifetime = false //should be false
            };

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            ClaimsPrincipal principal = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        private async Task<List<Claim>> handleUserClaimsAsync(ApplicationUser user)
        {
            var roles = await _unitOfWork.UserRepository.UserManager.GetRolesAsync(user);
            var claims = new List<Claim>()
            {
                new Claim("UserName", user.UserName),
                new Claim("Id", user.Id.ToString()),
                new Claim("Email", user.Email),
            };
            // roles register to claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }
    }
}
