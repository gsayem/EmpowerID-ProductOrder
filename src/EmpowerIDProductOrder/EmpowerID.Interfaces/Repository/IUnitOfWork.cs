using EmpowerID.Models;

namespace EmpowerID.Interfaces.Repository
{
    public interface IUnitOfWork
    {
        IRepository<TSet> GetRepository<TSet>() where TSet : class, IBaseModel;
        IRepositoryAsync<TSet> GetRepositoryAsync<TSet>() where TSet : class, IBaseModel;
        Task SaveChangesAsync(CancellationToken cancellationToken);
        void DetachAll<TSet>() where TSet : class, IBaseModel;
        void DetachAllAsync<TSet>() where TSet : class, IBaseModel;
        void SaveChanges();
    }
}
