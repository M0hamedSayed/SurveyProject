using Microsoft.EntityFrameworkCore.Storage;
using Survey.Domain.Interfaces.Repositories;
using Survey.Infrastructure.DatabaseContext;

namespace Survey.Infrastructure.Repositories
{
    public class UnitOfWork (ApplicationDbContext dbContext, IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository) : IUnitOfWork
    {
        private readonly ApplicationDbContext _context = dbContext;
        public IUserRepository UserRepository => userRepository;
        public IRefreshTokenRepository RefreshTokenRepository => refreshTokenRepository;

        public IDbContextTransaction BeginTransaction()
        {
            return _context.Database.BeginTransaction();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public void Commit()
        {
            _context.Database.CommitTransaction();
        }

        public async Task CommitAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public void RollBack()
        {
            _context.Database.RollbackTransaction();
        }

        public async Task RollBackAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        public async Task Complete()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose() => _context.Dispose();
    }
}
