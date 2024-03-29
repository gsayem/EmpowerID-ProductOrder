using EmpowerID.Infrastructure.Configuration;
using EmpowerID.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProductOrderApp.Configuration
{
    public static class AppSettingsConfiguration
    {
        private static string EncryptionSalt = "5FC65E33-F190-40C2-AFFE-C1191D7CA445";
        public static IServiceCollection AddAppSettingsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ConfigAppSettings>(options =>
            {
                if (configuration.GetValue<bool>("IsEncrypted"))
                {
                    options.ConnString = configuration.GetValue<string>("ConnString").ToDecrypt(EncryptionSalt);
                    options.ConnStringSecondary = configuration.GetValue<string>("ConnStringSecondary").ToDecrypt(EncryptionSalt);
                    options.ApplicationId = configuration.GetValue<string>("ApplicationId").ToDecrypt(EncryptionSalt);
                    options.ClientSecret = configuration.GetValue<string>("ClientSecret").ToDecrypt(EncryptionSalt);
                    options.DataFactoryName = configuration.GetValue<string>("DataFactoryName").ToDecrypt(EncryptionSalt);
                    options.DataFactoryResourceGroupName = configuration.GetValue<string>("DataFactoryResourceGroupName").ToDecrypt(EncryptionSalt);
                    options.TenantId = configuration.GetValue<string>("TenantId").ToDecrypt(EncryptionSalt);
                    options.SubscriptionId = configuration.GetValue<string>("SubscriptionId").ToDecrypt(EncryptionSalt);
                    options.DataFactoryPipeLineName = configuration.GetValue<string>("DataFactoryPipeLineName").ToDecrypt(EncryptionSalt);
                    options.SearchServiceEndPoint = configuration.GetValue<string>("SearchServiceEndPoint").ToDecrypt(EncryptionSalt);
                    options.QueryApiKey = configuration.GetValue<string>("QueryApiKey").ToDecrypt(EncryptionSalt);
                    options.SearchIndexName = configuration.GetValue<string>("SearchIndexName").ToDecrypt(EncryptionSalt);
                }
                else
                {
                    options.ConnString = configuration.GetValue<string>("ConnString");
                    options.ConnStringSecondary = configuration.GetValue<string>("ConnStringSecondary");
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
                }

            });

            return services;
        }
    }
}

