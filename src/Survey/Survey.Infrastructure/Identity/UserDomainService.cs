using Microsoft.AspNetCore.Identity;
using Survey.Domain.Events.Dispatcher;
using Survey.Domain.Exceptions;
using Survey.Domain.Models.Identity;

namespace Survey.Infrastructure.Identity
{
    public class UserDomainService (UserManager<ApplicationUser> userManager, IDomainEventDispatcher domainEventDispatcher )
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IDomainEventDispatcher _domainEventDispatcher = domainEventDispatcher;

        public async Task<Guid> RegisterUserAsync(ApplicationUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new DomainException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            return user.Id;
        }
    }
}
