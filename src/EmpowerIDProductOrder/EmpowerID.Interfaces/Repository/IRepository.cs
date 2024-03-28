
using EmpowerID.Models;
using System.Linq.Expressions;

namespace EmpowerID.Interfaces.Repository
{
    public interface IRepository<TEntity> where TEntity : class, IBaseModel
    {
        void Add(TEntity entity);
        void Remove(TEntity entity);
        void Update(TEntity entity);

        IList<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate);
        IList<TEntity> GetAll();
        IList<TEntity> GetAllFromRawSql(string sql);
        int Count();
        int Count(Expression<Func<TEntity, bool>> predicate);
        TEntity Find(Expression<Func<TEntity, bool>> predicate);
        TEntity GetById(string id);

        void Detach(TEntity entity);
        void DetachAll();
    }
}
