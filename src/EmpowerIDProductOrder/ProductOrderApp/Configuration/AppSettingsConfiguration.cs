using EmpowerID.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProductOrderApp.Configuration
{
    public static class AppSettingsConfiguration
    {
        public static IServiceCollection AddAppSettingsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ConfigAppSettings>(options =>
            {
                options.ConnString = configuration.GetValue<string>("ConnString");
            });

            return services;
        }
    }
}

