using EmpowerID.Interfaces.Repository;
using EmpowerID.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EmpowerID.Repository
{
    public class UOWRepository<TEntity> : IRepository<TEntity>, IRepositoryAsync<TEntity> where TEntity : class, IBaseModel
    {
        public UOWRepository(IDataContext dataContext)
        {
            this.DataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public IDataContext DataContext { get; }

        public virtual void Add(TEntity entity)
        {
            DataContext.Set<TEntity>().Add(entity);
        }

        public virtual void Remove(TEntity entity)
        {
            DataContext.Entry(entity).State = EntityState.Deleted;
        }

        public virtual IList<TEntity> GetAll()
        {
            return DataContext.Set<TEntity>().ToList();
        }

        public virtual void Update(TEntity entity)
        {
            DataContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual int Count()
        {
            return DataContext.Set<TEntity>().Count();
        }

        public virtual TEntity Find(Expression<Func<TEntity, bool>> predicate)
        {
            return DataContext.Set<TEntity>().SingleOrDefault(predicate);
        }

        public virtual IList<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate)
        {
            return DataContext.Set<TEntity>().Where(predicate).ToList();
        }

        public virtual void SaveChanges()
        {
            DataContext.SaveChanges();
        }

        public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await DataContext.Set<TEntity>().AddAsync(entity, cancellationToken);
        }
        public virtual async Task AddRangeAsync(CancellationToken cancellationToken, params TEntity[] entities)
        {
            await DataContext.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
        }

        public virtual async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken)
        {
            DataContext.Entry(entity).State = EntityState.Deleted;
            await Task.CompletedTask;
        }

        public virtual async Task<IList<TEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await DataContext.Set<TEntity>().ToListAsync(cancellationToken);
        }

        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            DataContext.Entry(entity).State = EntityState.Modified;
            await Task.CompletedTask;
        }

        public virtual async Task<int> CountAsync(CancellationToken cancellationToken)
        {
            return await DataContext.Set<TEntity>().CountAsync(cancellationToken);
        }

        public virtual async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return await DataContext.Set<TEntity>().SingleOrDefaultAsync(predicate, cancellationToken);
        }


        public virtual async Task<IList<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return await DataContext.Set<TEntity>().Where(predicate).ToListAsync(cancellationToken);
        }

        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return DataContext.Set<TEntity>().Count(predicate);
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return await DataContext.Set<TEntity>().CountAsync(predicate, cancellationToken);
        }


        public int Count(string sql, params object[] parameters)
        {
            return DataContext.Set<TEntity>().FromSqlRaw(sql, parameters).Count();
        }

        public void Detach(TEntity entity)
        {
            DataContext.Entry(entity).State = EntityState.Detached;
        }

        public void DetachAll()
        {
            foreach (var entry in DataContext.GetChangeTrackerEntries())
            {
                if (entry.Entity != null)
                {
                    entry.State = EntityState.Detached;
                }
            }
        }


        public TEntity GetById(string id)
        {
            return DataContext.Set<TEntity>().SingleOrDefault(/*i => i.Id == id*/);
        }

        public async Task<TEntity> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            return await DataContext.Set<TEntity>().SingleOrDefaultAsync(/*i => i.Id == id,*/ cancellationToken);
        }
    }
}
