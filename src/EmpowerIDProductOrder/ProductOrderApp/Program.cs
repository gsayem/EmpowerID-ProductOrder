using EmpowerID.Common;
using EmpowerID.Common.Extentions;
using EmpowerID.Interfaces.Services;
using EmpowerID.Repository;
using EmpowerID.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProductOrderApp.Configuration;

namespace ProductOrderApp
{
    internal class Program
    {
        static Application application;
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
                    services.AddDbContext<SecondaryEmpowerIDDBContext>((provider, options) =>
                    {
                        var _connectionString = context.Configuration.GetValue<string>("ConnStringSecondary");
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
                    services.AddTransient<ICategoryService, CategoryService>();
                    services.AddTransient<IProductService, ProductService>();
                    services.AddTransient<IOrderService, OrderService>();

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
                    application = scope.ServiceProvider.GetRequiredService<Application>();
                    await application.StartApplication();
                }
            }
            Console.WriteLine("Done. Thank you.");

        }

        private static async void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Unhandled Exception Occured.\n");
            Console.WriteLine($"Exception details:{e.ExceptionObject}\n");
            Console.ResetColor();

            Console.WriteLine("\n\nStating the application itself.\n");
            await application.StartApplication();
        }
    }
}
