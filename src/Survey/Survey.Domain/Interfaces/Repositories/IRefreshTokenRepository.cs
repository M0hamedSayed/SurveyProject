using Survey.Domain.Models.Identity;

namespace Survey.Domain.Interfaces.Repositories
{
    public interface IRefreshTokenRepository : IGenericRepositoryAsync<UserRefreshTokens>
    {
    }
}
