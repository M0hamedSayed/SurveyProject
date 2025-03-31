using Microsoft.AspNetCore.Http;
using Survey.Application.Interfaces;
using Survey.Domain.Interfaces.Repositories;
using Survey.Domain.Models.Identity;

namespace Survey.Application.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public CurrentUserService(IHttpContextAccessor contextAccessor, IUnitOfWork unitOfWork)
        {
            _contextAccessor = contextAccessor;
            _unitOfWork = unitOfWork;
        }
        public async Task<(ApplicationUser? user, IList<string> roles)> GetCurrentUserWithRolesAsync()
        {
            var user = await GetUserAsync();
            IList<string>? roles = await _unitOfWork.UserRepository.UserManager.GetRolesAsync(user);
            return (user, roles);
        }

        public async Task<ApplicationUser> GetUserAsync()
        {
            var userId = GetUserId();
            var user = await _unitOfWork.UserRepository.UserManager.FindByIdAsync(userId.ToString());
            return user is null ? throw new UnauthorizedAccessException() : user;
        }

        public Guid GetUserId()
        {
            var userId = _contextAccessor?.HttpContext?.User?.Claims?.SingleOrDefault(claim => claim.Type == "Id")?.Value;
            if (userId == null || !Guid.TryParse(userId.ToString(), out var id)) throw new UnauthorizedAccessException();
            return id;
        }
    }
}
