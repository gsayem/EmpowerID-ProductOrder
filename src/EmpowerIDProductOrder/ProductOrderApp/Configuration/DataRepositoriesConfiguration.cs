using EmpowerID.Interfaces.Repository;
using EmpowerID.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace ProductOrderApp.Configuration
{
    public static class DataRepositoriesConfiguration
    {
        public static IServiceCollection AddDataRepositories(this IServiceCollection services)
        {
            services.AddTransient<IDataContext, EmpowerIDDBContext>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient(typeof(IRepositoryAsync<>), typeof(Repository<>));
            return services;
        }
    }
}
