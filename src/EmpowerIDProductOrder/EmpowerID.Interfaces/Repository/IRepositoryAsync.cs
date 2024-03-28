using EmpowerID.Models;
using System.Linq.Expressions;

namespace EmpowerID.Interfaces.Repository
{
    public interface IRepositoryAsync<TEntity> where TEntity : class, IBaseModel
    {
        Task AddAsync(TEntity entity, CancellationToken cancellationToken);
        Task AddRangeAsync(CancellationToken cancellationToken, params TEntity[] entities);
        Task RemoveAsync(TEntity entity, CancellationToken cancellationToken);
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);
        Task<IList<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);
        Task<IList<TEntity>> GetAllAsync(CancellationToken cancellationToken);
        Task<IList<TEntity>> GetAllFromRawSqlAsync(string sql, CancellationToken cancellationToken);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);
        Task<TEntity> GetByIdAsync(string id, CancellationToken cancellationToken);
        void Detach(TEntity entity);
        void DetachAll();
    }
}
