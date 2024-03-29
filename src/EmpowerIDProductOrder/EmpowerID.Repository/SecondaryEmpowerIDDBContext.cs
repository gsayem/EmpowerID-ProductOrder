using EmpowerID.Common;
using EmpowerID.Common.Enums;
using EmpowerID.Common.Extentions;
using EmpowerID.Infrastructure.Configuration;
using EmpowerID.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;

namespace EmpowerID.Repository
{
    public partial class SecondaryEmpowerIDDBContext : DbContext, IDataContext
    {
        private readonly IOptions<ConfigAppSettings> _configuration;
        private DbContextOptionsBuilder _optionsBuilder;
        protected string _connectionString { private set; get; }
        public SecondaryEmpowerIDDBContext(DbContextOptions<SecondaryEmpowerIDDBContext> options, IOptions<ConfigAppSettings> configuration) : base(options)
        {
            _configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _connectionString = _configuration.Value.ConnStringSecondary;
            if (_connectionString.IsNotNull())
            {
                optionsBuilder.UseSqlServer(_connectionString, options =>
                {
                    options.CommandTimeout(AppConstant.DB_COMMAND_TIME_OUT_IN_SEC);
                    options.EnableRetryOnFailure(AppConstant.DB_ENABLE_RETRY_ON_FAILURE);
                }).UseLazyLoadingProxies();
            }
            base.OnConfiguring(optionsBuilder);
            _optionsBuilder = optionsBuilder;
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
            //ProductOrderModelBuilder(modelBuilder);
        }

        public void ChangeDatabase(DatabaseConnection databaseConnection)
        {
            switch (databaseConnection)
            {
                case DatabaseConnection.Primary:
                    _connectionString = _configuration.Value.ConnString;
                    break;
                case DatabaseConnection.Secondary:
                    _connectionString = _configuration.Value.ConnStringSecondary;
                    break;
                default:
                    break;
            }
            if (_connectionString.IsNotNull())
            {
                _optionsBuilder.UseSqlServer(_connectionString, options =>
                {
                    options.CommandTimeout(AppConstant.DB_COMMAND_TIME_OUT_IN_SEC);
                    options.EnableRetryOnFailure(AppConstant.DB_ENABLE_RETRY_ON_FAILURE);
                }).UseLazyLoadingProxies();
            }
            base.OnConfiguring(_optionsBuilder);
        }
    }
}
