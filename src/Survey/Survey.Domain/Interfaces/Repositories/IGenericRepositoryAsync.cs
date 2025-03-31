using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Shared.Consts;

namespace Survey.Domain.Interfaces.Repositories
{
    public interface IGenericRepositoryAsync<T> where T : class
    {
        // tracking
        IQueryable<T> GetTableNoTracking();
        IQueryable<T> GetTableAsTracking();
        // get
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(int skip, int take);

        public Task<T?> GetByIdAsync(Guid id);
        //find
        public Task<T?> FindAsync(Expression<Func<T, bool>> criteria, string[]? includes = null);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, string[]? includes = null);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, string[]? includes, int take, int skip);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, string[]? includes = null, int? take = null, int? skip = null,
            Expression<Func<T, object>>? orderBy = null, string orderByDirection = OrderBy.Ascending);
        //add
        Task<T> AddAsync(T entity);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        // update
        T Update(T entity);
        IEnumerable<T> UpdateRange(IEnumerable<T> entities);
        // delete
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        // count
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> criteria);
        // save changes
        Task SaveChangesAsync();
        void Attach(T entity);
        EntityEntry<T> Entry(T entity);
        void AttachRange(IEnumerable<T> entities);
    }
}
