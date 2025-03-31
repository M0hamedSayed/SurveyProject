using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Shared.Helpers;
using Survey.Application.Helpers;
using Survey.Application.Interfaces;
using Survey.Domain.Interfaces.Repositories;
using Survey.Domain.Models.Identity;
using Survey.Domain.ValueObjects.Identity;

namespace Survey.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly TokenSettings _tokenSettings;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ITokenService _tokenService;

        public AuthenticationService(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, IOptions<TokenSettings> tokenSettings, IHostEnvironment hostEnvironment, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _tokenSettings = tokenSettings.Value;
            _hostEnvironment = hostEnvironment;
            _tokenService = tokenService;
        }

        public async Task<ApplicationUser> GenerateUserTokensAsync(ApplicationUser user)
        {
            string accessToken = await _tokenService.GenerateGwtToken(user);
            string refreshToken = _tokenService.GenerateRefreshToken();

            string? userIp = GetUserIp();
            if (_hostEnvironment.IsDevelopment())
            {
                userIp = "41.46.58.182";
            }
            GeoLocation? userMetaData = null;

            if (userIp is not null)
            {
                userMetaData = await GetLocationFromIP(userIp);
            }

            DateTime rTokenExpiryDate = DateTime.UtcNow.AddDays(_tokenSettings.RefreshTokenExpireDate);

            var metaData = UserMetaData.Of(GetUserAgent(), userIp, userMetaData?.country, userMetaData?.countryCode, userMetaData?.city, userMetaData?.timezone, userMetaData?.lat, userMetaData?.lon);

            var userRefreshToken = new UserRefreshTokens(user.Id, accessToken, refreshToken, DateTime.UtcNow.AddDays(_tokenSettings.RefreshTokenExpireDate), metaData);

            user.addRefreshToken(metaData, accessToken, refreshToken, rTokenExpiryDate);
            await _unitOfWork.UserRepository.UserManager.UpdateAsync(user);
            // add refreshtoken to cookies
            _contextAccessor?.HttpContext?.Response.Cookies.Append(
                "refreshToken",
                refreshToken,
                options: new CookieOptions { Expires = rTokenExpiryDate, HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict }
            );
            return user;
        }

        public async Task<string> RefreshTokenAsync()
        {
            var accessToken = _contextAccessor?.HttpContext?.Request.Headers?.Authorization.SingleOrDefault()?.Split(" ")[1];
            var refreshtoken = _contextAccessor?.HttpContext?.Request?.Cookies["refreshToken"];
            if (accessToken is null || refreshtoken is null) throw new UnauthorizedAccessException("Invalid Token");

            var principal = _tokenService.GetPrincipalFromJwtToken(accessToken);

            if (principal is null) throw new UnauthorizedAccessException("Invalid Token");

            string? id = principal.FindFirstValue("Id");

            if (!Guid.TryParse(id, out var userId)) throw new UnauthorizedAccessException("Invalid Token");

            // get user refresh token
            var user = await _unitOfWork.UserRepository.UserManager.FindByIdAsync(userId.ToString());
            if (user is not null)
                await _unitOfWork.UserRepository.Entry(user).Collection(u => u.RefreshTokens).Query().Where(rt => rt.RefreshToken == refreshtoken).LoadAsync();

            var rToken = user?.RefreshTokens.FirstOrDefault();

            if (user is null || rToken is null || (rToken.IsRevoked && rToken is not null))
            {
                _contextAccessor?.HttpContext?.Response.Cookies.Delete("refreshToken");
                throw new UnauthorizedAccessException("Invalid Refresh Token");
            }

            if (rToken.ExpiryDate <= rToken.AddedTime)
            {
                //rToken.IsRevoked = true;
                user.RevokeRefreshToken(rToken);
                await _unitOfWork.Complete();
                _contextAccessor?.HttpContext?.Response.Cookies.Delete("refreshToken");
                throw new UnauthorizedAccessException("Invalid Refresh Token");
            }

            var newAccessToken = await _tokenService.GenerateGwtToken(user);

            rToken.AssignNewAccessToken(newAccessToken);
            await _unitOfWork.Complete();

            return newAccessToken;
        }

        public async Task<bool> Logout()
        {
            // GET Refresh token
            var httpContext = _contextAccessor?.HttpContext;
            var refreshToken = httpContext?.Request.Cookies["refreshToken"];

            if (refreshToken is not null)
            {
                var rToken = await _unitOfWork.RefreshTokenRepository.FindAsync(rt => rt.RefreshToken == refreshToken);
                if (rToken is null || rToken.IsRevoked) { return false; }
                if (rToken is not null)
                {
                    rToken.Revoke();
                    await _unitOfWork.Complete();
                }
            }
            httpContext?.Response.Cookies.Delete("refreshToken");
            return true;
        }


        private string? GetUserIp() => _contextAccessor?.HttpContext?.Connection.RemoteIpAddress?.ToString();

        private string? GetUserAgent() => _contextAccessor?.HttpContext?.Request.Headers.UserAgent.ToString();

        private async Task<GeoLocation?> GetLocationFromIP(string ip)
        {
            // Example using an IP Geolocation API (install "IpStack.Net" or "MaxMind.GeoIP2")
            try
            {
                using var client = new HttpClient();
                var response = await client.GetStringAsync($"http://ip-api.com/json/{ip}");
                var result = JsonSerializer.Deserialize<GeoLocation>(response);
                return result;
            }
            catch
            {
                return null;
            }
        }
    }
}
