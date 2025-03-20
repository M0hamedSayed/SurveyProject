
using Microsoft.EntityFrameworkCore.Storage;

namespace Survey.Domain.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        //repositories
        IUserRepository UserRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }
        // transactions
        IDbContextTransaction BeginTransaction();
        void Commit();
        void RollBack();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitAsync();
        Task RollBackAsync();
        // save changes
        Task Complete();
    }
}
