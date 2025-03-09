using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MMA.Domain;

namespace MMA.Service
{
    partial class DbRepository : IDbRepository
    {
        private readonly MMADbContext _dbContext;

        public DbRepository(MMADbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private static void InitBaseEntity<T>(T entity) where T : class
        {
            if (entity is BaseEntity bEntity)
            {
                bEntity.CreatedBy = bEntity.ModifiedBy = RuntimeContext.CurrentUser?.Id ?? CoreConstant.SYSTEM_ACCOUNT_ID;
            }
            else if (entity is BaseInfo baseInfo)
            {
                baseInfo.CreatedBy = baseInfo.ModifiedBy = RuntimeContext.CurrentUser?.Id ?? CoreConstant.SYSTEM_ACCOUNT_ID;
            }
        }

        private static void UpdateBaseEntity<T>(T entity) where T : class
        {
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.ModifiedBy = RuntimeContext.CurrentUser?.Id ?? CoreConstant.SYSTEM_ACCOUNT_ID;
                baseEntity.ModifiedDate = DateTimeOffset.UtcNow;
            }
        }

        public IQueryable<T> Queryable<T>(
            Func<IQueryable<T>, IQueryable<T>>? query = null,
            CancellationToken cancellationToken = default) where T : class
        {
            var dbSet = _dbContext.Set<T>().AsQueryable();

            if (query != null)
            {
                dbSet = query(dbSet);
            }

            return dbSet;
        }

        public async Task ActionInTransaction(Func<Task> action)
        {
            try
            {
                using (var trans = await _dbContext.Database.BeginTransactionAsync())
                {
                    await action();
                    await trans.CommitAsync();
                }
            }
            catch
            {
                // Optionally, handle exceptions here, log them, etc.
                throw;
            }
        }

        public async Task<T> AddAsync<T>(T entity, bool clearTracker = false,
            CancellationToken cancellationToken = default, bool needSaveChange = true) where T : class
        {
            InitBaseEntity(entity: entity);
            var entry = await _dbContext.Set<T>().AddAsync(entity: entity, cancellationToken: cancellationToken);
            if (needSaveChange)
            {
                await _dbContext.SaveChangesAsync(acceptAllChangesOnSuccess: clearTracker, cancellationToken: cancellationToken);
            }
            return entry.Entity;
        }

        public async Task<int> AddRangeAsync<T>(IEnumerable<T> entities, bool clearTracker = false,
            CancellationToken cancellationToken = default, bool needSaveChange = true) where T : class
        {
            foreach (var entity in entities)
            {
                InitBaseEntity(entity: entity);
            }
            int result = 0;
            await _dbContext.Set<T>().AddRangeAsync(entities: entities, cancellationToken: cancellationToken);
            if (needSaveChange)
            {
                result = await _dbContext.SaveChangesAsync(acceptAllChangesOnSuccess: clearTracker, cancellationToken: cancellationToken);
            }
            return result;
        }

        public async Task<T> UpdateAsync<T>(T entity, bool clearTracker = false,
            CancellationToken cancellationToken = default, bool needSaveChange = true) where T : class
        {
            UpdateBaseEntity(entity: entity);
            var result = _dbContext.Entry(entity);

            if (result.State == EntityState.Detached)
            {
                result = _dbContext.Set<T>().Update(entity);
            }

            if (needSaveChange)
            {
                await _dbContext.SaveChangesAsync(acceptAllChangesOnSuccess: clearTracker, cancellationToken: cancellationToken);
            }
            return result.Entity;
        }

        public async Task<int> UpdateRangeAsync<T>(IEnumerable<T> entities, bool clearTracker = false,
            CancellationToken cancellationToken = default, bool needSaveChange = true) where T : class
        {
            int result = 0;
            foreach (var entity in entities)
            {
                UpdateBaseEntity<T>(entity: entity);
                var entry = _dbContext.Entry(entity);

                if (entry.State == EntityState.Detached)
                {
                    entry = _dbContext.Set<T>().Update(entity);
                }
            }
            if (needSaveChange)
            {
                result = await _dbContext.SaveChangesAsync(acceptAllChangesOnSuccess: clearTracker, cancellationToken: cancellationToken);
            }

            return result;
        }

        public async Task<int> DeleteAsync<T>(T entity, bool clearTracker = false,
            CancellationToken cancellationToken = default, bool needSaveChange = true) where T : class
        {
            int result = 0;
            _dbContext.Set<T>().Remove(entity);
            if (needSaveChange)
            {
                result = await _dbContext.SaveChangesAsync(clearTracker, cancellationToken);
            }
            return result;
        }

        public async Task<int> DeleteRangeAsync<T>(IEnumerable<T> entities, bool clearTracker = false,
            CancellationToken cancellationToken = default, bool needSaveChange = true) where T : class
        {
            int result = 0;
            _dbContext.Set<T>().RemoveRange(entities);
            if (needSaveChange)
            {
                result = await _dbContext.SaveChangesAsync(clearTracker, cancellationToken);
            }
            return result;
        }

        public async Task<T?> FindForUpdateAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) where T : class
        {
            return await _dbContext.Set<T>().AsTracking().FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<T?> FindAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) where T : class
        {
            return await _dbContext.Set<T>().FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<List<T>> GetAsync<T>(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default) where T : class
        {
            if (predicate == null)
            {
                return await _dbContext.Set<T>().ToListAsync(cancellationToken);
            }
            return await _dbContext.Set<T>().Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<List<R>> GetWithSelectAsync<T, R>(Expression<Func<T, R>> selector, Expression<Func<T, bool>>? predicate = null,
            CancellationToken cancellationToken = default) where T : class
        {
            if (predicate == null)
            {
                return await _dbContext.Set<T>().Select(selector).ToListAsync(cancellationToken);
            }
            return await _dbContext.Set<T>().Where(predicate).Select(selector).ToListAsync(cancellationToken);
        }

        public async Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) where T : class
        {
            return await _dbContext.Set<T>().AnyAsync(predicate, cancellationToken);
        }
    }
}
