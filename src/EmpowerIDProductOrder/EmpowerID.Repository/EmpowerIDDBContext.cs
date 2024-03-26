using EmpowerID.Common;
using EmpowerID.Common.Extentions;
using EmpowerID.Infrastructure.Configuration;
using EmpowerID.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;

namespace EmpowerID.Repository
{
    public partial class EmpowerIDDBContext : DbContext, IDataContext
    {
        private readonly IOptions<ConfigAppSettings> _configuration;
        protected string _connectionString { private set; get; }
        public EmpowerIDDBContext(DbContextOptions<EmpowerIDDBContext> options, IOptions<ConfigAppSettings> configuration) : base(options)
        {
            _configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _connectionString = _configuration.Value.ConnString;
            if (_connectionString.IsNotNull())
            {
                optionsBuilder.UseSqlServer(_connectionString, options =>
                {
                    options.CommandTimeout(AppConstant.DB_COMMAND_TIME_OUT_IN_SEC);
                    options.EnableRetryOnFailure(AppConstant.DB_ENABLE_RETRY_ON_FAILURE);
                }).UseLazyLoadingProxies();
            }
            base.OnConfiguring(optionsBuilder);

        }
        public override DbSet<TEntity> Set<TEntity>()
        {
            return base.Set<TEntity>();
        }

        public override EntityEntry<TEntity> Entry<TEntity>(TEntity entity)
        {
            return base.Entry(entity);
        }

        public override EntityEntry Entry(object entity)
        {
            return base.Entry(entity);
        }
        public IEnumerable<EntityEntry> GetChangeTrackerEntries()
        {
            return ChangeTracker.Entries();
        }
        private string GetDbConnectionString()
        {
            var dbName = "In Memory ?";

            try
            {
                dbName = base.Database.GetDbConnection().ConnectionString;
            }
            catch
            {
                // dont care about the exception here, just want the connection string or "In Memory ? for Unit test"
            }

            return dbName;
        }

        public override int SaveChanges()
        {
            int returnValue = -1;
            var strategy = this.Database.CreateExecutionStrategy();
            strategy.Execute(() =>
            {
                using (var dbContextTransaction = base.Database.BeginTransaction())
                {
                    try
                    {
                        returnValue = base.SaveChanges();
                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        returnValue = -1;
                        dbContextTransaction.Rollback();
                        throw;
                    }
                }
            });
            return returnValue;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            int returnValue = -1;
            var strategy = this.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var dbContextTransaction = await base.Database.BeginTransactionAsync(cancellationToken))
                {
                    try
                    {
                        returnValue = await base.SaveChangesAsync(cancellationToken);
                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        returnValue = -1;
                        dbContextTransaction.Rollback();
                        throw;
                    }
                }
            });
            return returnValue;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            CategoryModelBuilder(modelBuilder);
            OrderModelBuilder(modelBuilder);
            ProductModelBuilder(modelBuilder);
            ProductOrderModelBuilder(modelBuilder);
        }
    }
}
