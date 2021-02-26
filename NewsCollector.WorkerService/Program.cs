using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NewsCollector.Core;
using NewsCollector.Core.Services;
using NewsCollector.Data;
using NewsCollector.Services;
using NewsCollector.WorkerService.Helpers;
using NewsCollector.WorkerService.Services;
using System;
using System.Configuration;
using System.Threading.Tasks;
using static NewsCollector.WorkerService.Helpers.CollectNewsByKeywordHelper;
using static NewsCollector.WorkerService.Helpers.CollectNewsBySourceHelper;

namespace NewsCollector.WorkerService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {

                    IConfigurationRoot configuration = new ConfigurationBuilder()
                      .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                      .AddJsonFile("appsettings.json")
                      .Build();

                    services.AddDbContext<NewsCollectorDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("mydb")), optionsLifetime: ServiceLifetime.Transient);

                    services.AddScoped<IUnitOfWork, UnitOfWork>();
                    services.AddTransient<ISourceService, SourceService>();
                    services.AddTransient<IKeywordService, KeywordService>();
                    services.AddTransient<INewsKeywordService, NewsKeywordService>();
                    services.AddTransient<INewsService, NewsService>();

                    services.AddScoped<ICollectNewsByKeywordHelper, CollectNewsByKeywordHelper>();
                    services.AddScoped<ICollectNewsBySourceHelper, CollectNewsBySourceHelper>();

                    services.AddHostedService<TimedWorker>();

                })
                .Build();

            await host.StartAsync();
            await host.WaitForShutdownAsync();
        }
    }
}
