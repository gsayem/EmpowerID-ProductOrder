using EmpowerID.Infrastructure.Configuration;
using EmpowerID.Interfaces.Services.Products;
using EmpowerID.Repository;
using EmpowerID.Services.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductOrderApp.Configuration;
using EmpowerID.Common.Extentions;
using EmpowerID.Common;
using EmpowerID.Seeds;
namespace ProductOrderApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            using (IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(cfg =>
                {
                    cfg.AddJsonFile("appsettings.json");
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<EmpowerIDDBContext>((provider, options) =>
                    {
                        var _connectionString = context.Configuration.GetValue<string>("ConnString");
                        if (_connectionString.IsNotNull())
                        {
                            options.UseSqlServer(_connectionString, options =>
                            {
                                options.CommandTimeout(AppConstant.DB_COMMAND_TIME_OUT_IN_SEC);
                                options.EnableRetryOnFailure(AppConstant.DB_ENABLE_RETRY_ON_FAILURE);
                            });
                        }
                    }, ServiceLifetime.Transient);
                    services.AddAppSettingsConfiguration(context.Configuration);
                    services.AddDataRepositories();
                    services.AddTransient<IProductService, ProductService>();
                    services.AddScoped<Application>();
                })
                .ConfigureLogging((context, cfg) =>
                {
                    cfg.ClearProviders();
                    cfg.AddConfiguration(context.Configuration.GetSection("Logging"));
                    cfg.AddConsole();
                })
                .Build()
                )
            {
                using (IServiceScope scope = host.Services.CreateScope())
                {
                    Application p = scope.ServiceProvider.GetRequiredService<Application>();
                    await p.MyLogic();
                }
            }
            Console.WriteLine("Done.");

        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
