using Microsoft.EntityFrameworkCore;
using Survey.Domain.Interfaces;
using Survey.Domain.Interfaces.Repositories;
using Survey.Domain.Models.Survey;

namespace Survey.Infrastructure.Seeding
{
    public static class DataSeed
    {
        public static async Task SeedAsync(IUserDomainService domainService, IUnitOfWork unitOfWork)
        {
            if (domainService == null) return;

            var usersCount = await unitOfWork.UserRepository.UserManager.Users.AsQueryable().CountAsync();
            if (usersCount <= 0)
            {
                var user = await domainService.RegisterUserAsync("Mohamed Sayed", "admin@admin.com", "Ms@123456", "Admin", null, true);
                await domainService.RegisterUserAsync("Mohamed Sayed 1", "mohamed.sayed.atiaa@gmail.com", "Ms@123456", "User", user.Id, true);
                await domainService.RegisterUserAsync("Mohamed Sayed 2", "mohamedsayed.alqemam@gmail.com", "Ms@123456", "User", user.Id, true);
            }
            var typesCount = await unitOfWork.SurveyTypeRepository.CountAsync();
            if (typesCount <= 0) 
            {
                List<SurveyType> types = new List<SurveyType>()
            {
                SurveyType.Create(Guid.NewGuid(), "General", "عام"),
                SurveyType.Create(Guid.NewGuid(), "Test", "تست"),
                SurveyType.Create(Guid.NewGuid(), "HR Resources", "موارد بشريه"),
            };
                var surveyTypes = await unitOfWork.SurveyTypeRepository.AddRangeAsync(types);
                await unitOfWork.Complete();
            }
            
        }
    }
}
