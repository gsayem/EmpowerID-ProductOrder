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
                options.ApplicationId = configuration.GetValue<string>("ApplicationId");
                options.ClientSecret = configuration.GetValue<string>("ClientSecret");
                options.DataFactoryName = configuration.GetValue<string>("DataFactoryName");
                options.DataFactoryResourceGroupName = configuration.GetValue<string>("DataFactoryResourceGroupName");
                options.TenantId = configuration.GetValue<string>("TenantId");
                options.SubscriptionId = configuration.GetValue<string>("SubscriptionId");
                options.DataFactoryPipeLineName = configuration.GetValue<string>("DataFactoryPipeLineName");
                options.SearchServiceEndPoint = configuration.GetValue<string>("SearchServiceEndPoint");
                options.QueryApiKey = configuration.GetValue<string>("QueryApiKey");
                options.SearchIndexName = configuration.GetValue<string>("SearchIndexName");
            });

            return services;
        }
    }
}

