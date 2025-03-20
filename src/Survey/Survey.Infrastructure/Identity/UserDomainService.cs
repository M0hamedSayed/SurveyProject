using System.Data;
using Survey.Domain.Events;
using Survey.Domain.Events.Dispatcher;
using Survey.Domain.Exceptions;
using Survey.Domain.Interfaces;
using Survey.Domain.Interfaces.Repositories;
using Survey.Domain.Models.Identity;

namespace Survey.Infrastructure.Identity
{
    public class UserDomainService (IDomainEventDispatcher domainEventDispatcher, IUnitOfWork unitOfWork) : IUserDomainService
    {
        private readonly IDomainEventDispatcher _domainEventDispatcher = domainEventDispatcher;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<ApplicationUser> RegisterUserAsync(string fullName, string email, string password, string role, Guid? managerId, bool emailConfirmed = false)
        {
            // start transaction
            using var transactions = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var user = await CreateUserAsync(fullName, email, password, managerId, emailConfirmed);

                // assign role to user
                await AddRoleAsync(user, role);

                // generate email confirmation token and fire confirm email event
                if (!emailConfirmed)
                    await SendEmailConfirmEventAsync(user);

                // commit transaction
                await transactions.CommitAsync();
                return user;
            }
            catch
            {
                await transactions.RollbackAsync();
                throw new DomainException("user creation failed, please try again later");
            }
        }

        private async Task<ApplicationUser> CreateUserAsync(string fullName, string email, string password, Guid? managerId, bool emailConfirmed = false)
        {
            var user = new ApplicationUser(fullName, email) { EmailConfirmed = emailConfirmed };
            // assign manager
            user = await AssignManagerToUser(user, managerId);
            // handle last email confirmed
            if(!emailConfirmed)  user.HandleSendEmailDate(DateTime.UtcNow);

            var result = await _unitOfWork.UserRepository.UserManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new DomainException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }            

            return user;
        }

        private async Task<ApplicationUser> AssignManagerToUser(ApplicationUser user, Guid? managerId)
        {
            if (managerId.HasValue)
            {
                var manager = await _unitOfWork.UserRepository.UserManager.FindByIdAsync(managerId.Value.ToString());
                if (manager == null)
                {
                    throw new DomainException("Manager not found.");
                }

                user.AssignManager(manager); // assigns the manager
            }
            return user;
        }

        private async Task EnsureRoleExist( string roleName)
        {
            var roleExists = await _unitOfWork.UserRepository.RoleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                var createRoleResult = await _unitOfWork.UserRepository.RoleManager.CreateAsync(new ApplicationRole(roleName));
                if (!createRoleResult.Succeeded)
                {
                    throw new DomainException("Failed to create role.");
                }
            }
        }

        private async Task AddRoleAsync(ApplicationUser user,string roleName)
        {
            // validate role exist
            await EnsureRoleExist(roleName);
            // add role to this user
            var addToRoleResult = await _unitOfWork.UserRepository.UserManager.AddToRoleAsync(user, roleName);
            if (!addToRoleResult.Succeeded)
            {
                throw new DomainException("Failed to assign role to user.");
            }
        }

        private async Task SendEmailConfirmEventAsync(ApplicationUser user)
        {
            var confirmationToken = await _unitOfWork.UserRepository.UserManager.GenerateEmailConfirmationTokenAsync(user);

            var domainEvent = new ConfirmEmailEvent(user.Email, user.Id, confirmationToken);

            await _domainEventDispatcher.DispatchAsync(domainEvent);
        }
    }
}
