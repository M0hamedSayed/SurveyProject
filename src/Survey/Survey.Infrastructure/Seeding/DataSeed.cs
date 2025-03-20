using Microsoft.EntityFrameworkCore;
using Survey.Domain.Interfaces;
using Survey.Domain.Interfaces.Repositories;

namespace Survey.Infrastructure.Seeding
{
    public static class DataSeed
    {
        public static async Task SeedAsync (IUserDomainService domainService, IUnitOfWork unitOfWork)
        {
            if (domainService == null) return;

            var usersCount = await unitOfWork.UserRepository.UserManager.Users.AsQueryable().CountAsync();
            if (usersCount <= 0)
            {
                var user = await domainService.RegisterUserAsync("Mohamed Sayed", "admin@admin.com", "Ms@123456", "Admin", null, true);
                await domainService.RegisterUserAsync("Mohamed Sayed 1", "User1@admin.com", "Ms@123456", "User", user.Id, true);
                await domainService.RegisterUserAsync("Mohamed Sayed 2", "User2@admin.com", "Ms@123456", "User", user.Id, true);
            }
               
        }
    }
}
