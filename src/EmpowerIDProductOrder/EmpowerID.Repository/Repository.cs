using EmpowerID.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EmpowerID.Repository
{
    public class Repository<TEntity> : UOWRepository<TEntity> where TEntity : class, IBaseModel
    {
        public Repository(EmpowerIDDBContext dataContext, SecondaryEmpowerIDDBContext secondaryDataContext) : base(dataContext, secondaryDataContext)
        {
        }

        public override void Add(TEntity entity)
        {
            base.Add(entity);
            DataContext.SaveChanges();
        }

        public override async Task AddRangeAsync(CancellationToken cancellationToken, params TEntity[] entities)
        {
            await base.AddRangeAsync(cancellationToken, entities);
            DataContext.SaveChanges();
        }

        public override void Update(TEntity entity)
        {
            base.Update(entity);
            DataContext.SaveChanges();
        }

        public override void Remove(TEntity entity)
        {
            base.Remove(entity);
            DataContext.SaveChanges();
        }

        public override async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await base.AddAsync(entity, cancellationToken);
            await DataContext.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public override async Task<int> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await base.UpdateAsync(entity, cancellationToken);
            return await DataContext.SaveChangesAsync(cancellationToken);
        }

        public override async Task<int> RemoveAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await base.RemoveAsync(entity, cancellationToken);
            return await DataContext.SaveChangesAsync(cancellationToken);
        }

        public override async Task<IList<TEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await DataContext.Set<TEntity>().ToListAsync(cancellationToken);
        }

        public override async Task<int> CountAsync(CancellationToken cancellationToken)
        {
            return await DataContext.Set<TEntity>().CountAsync(cancellationToken);
        }

        public override async Task<IList<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return await DataContext.Set<TEntity>().Where(predicate).ToListAsync(cancellationToken);
        }

        public override async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return await DataContext.Set<TEntity>().SingleOrDefaultAsync(predicate, cancellationToken);
        }
    }
}
