
using Microsoft.EntityFrameworkCore;
using Survey.Domain.Interfaces.Repositories;
using Survey.Domain.Models.Identity;
using Survey.Infrastructure.DatabaseContext;

namespace Survey.Infrastructure.Repositories
{
    public class RefreshTokenRepository(ApplicationDbContext context) : GenericrepositoryAsync<UserRefreshTokens>(context), IRefreshTokenRepository
    {
        private readonly DbSet<UserRefreshTokens> _refreshTokens = context.Set<UserRefreshTokens>();
    }
}
