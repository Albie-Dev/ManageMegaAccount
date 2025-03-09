using System.Linq.Expressions;

namespace MMA.Service
{
    public interface IDbRepository
    {
        IQueryable<T> Queryable<T>(
            Func<IQueryable<T>, IQueryable<T>>? query = null,
            CancellationToken cancellationToken = default) where T : class;

        Task ActionInTransaction(Func<Task> action);

        Task<T> AddAsync<T>(T entity, bool clearTracker = false,
            CancellationToken cancellationToken = default, bool needSaveChange = true) where T : class;

        Task<int> AddRangeAsync<T>(IEnumerable<T> entities, bool clearTracker = false,
            CancellationToken cancellationToken = default, bool needSaveChange = true) where T : class;

        Task<T> UpdateAsync<T>(T entity, bool clearTracker = false,
            CancellationToken cancellationToken = default, bool needSaveChange = true) where T : class;

        Task<int> UpdateRangeAsync<T>(IEnumerable<T> entities, bool clearTracker = false,
            CancellationToken cancellationToken = default, bool needSaveChange = true) where T : class;

        Task<int> DeleteAsync<T>(T entity, bool clearTracker = false,
            CancellationToken cancellationToken = default, bool needSaveChange = true) where T : class;

        Task<int> DeleteRangeAsync<T>(IEnumerable<T> entities, bool clearTracker = false,
            CancellationToken cancellationToken = default, bool needSaveChange = true) where T : class;

        Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) where T : class;

        Task<List<T>> GetAsync<T>(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default) where T : class;

        Task<List<R>> GetWithSelectAsync<T, R>(Expression<Func<T, R>> selector, Expression<Func<T, bool>>? predicate = null,
            CancellationToken cancellationToken = default) where T : class;

        Task<T?> FindAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) where T : class;

        Task<T?> FindForUpdateAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) where T : class;
    }
}