using EmpowerID.Common.Enums;
using EmpowerID.Interfaces.Repository;
using EmpowerID.Models;

namespace EmpowerID.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private IDataContext dataContext;
        private readonly EmpowerIDDBContext _primaryDbContext;
        private readonly SecondaryEmpowerIDDBContext _secondaryDbContext;
        private readonly Dictionary<Type, object> repositories;

        public UnitOfWork(EmpowerIDDBContext primaryDataContext, SecondaryEmpowerIDDBContext secondaryDataContext)
        {
            _primaryDbContext = primaryDataContext;
            _secondaryDbContext = secondaryDataContext;
            this.dataContext = primaryDataContext;
            repositories = new Dictionary<Type, object>();
        }

        public void SaveChanges()
        {
            dataContext.SaveChanges();
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await dataContext.SaveChangesAsync(cancellationToken);
        }

        public IRepository<TSet> GetRepository<TSet>() where TSet : class, IBaseModel
        {
            if (repositories.ContainsKey(typeof(TSet)))
            {
                return repositories[typeof(TSet)] as IRepository<TSet>;
            }
            var repository = new UOWRepository<TSet>(_primaryDbContext, _secondaryDbContext);
            repositories.Add(typeof(TSet), repository);
            return repository;
        }

        public IRepositoryAsync<TSet> GetRepositoryAsync<TSet>() where TSet : class, IBaseModel
        {

            if (repositories.ContainsKey(typeof(TSet)))
            {
                return repositories[typeof(TSet)] as IRepositoryAsync<TSet>;
            }
            var repository = new UOWRepository<TSet>(_primaryDbContext, _secondaryDbContext);
            repositories.Add(typeof(TSet), repository);
            return repository;
        }

        public void DetachAll<TSet>() where TSet : class, IBaseModel
        {
            if (repositories.ContainsKey(typeof(TSet)))
            {
                (repositories[typeof(TSet)] as IRepository<TSet>)?.DetachAll();
            }
        }

        public void DetachAllAsync<TSet>() where TSet : class, IBaseModel
        {
            if (repositories.ContainsKey(typeof(TSet)))
            {
                (repositories[typeof(TSet)] as IRepositoryAsync<TSet>)?.DetachAll();
            }
        }

        public void ChangeDatabase(DatabaseConnection databaseConnection)
        {
            this.dataContext = databaseConnection switch
            {
                DatabaseConnection.Primary => _primaryDbContext,
                DatabaseConnection.Secondary => _secondaryDbContext,
                _ => _primaryDbContext,
            };
        }
    }
}
