using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NewsCollector.Core;
using NewsCollector.Core.Services;
using NewsCollector.Data;
using NewsCollector.Services;
using NewsCollector.WorkerService.Services;
using System;
using System.Configuration;
using System.Threading.Tasks;

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

                    var optionsBuilder = new DbContextOptionsBuilder<NewsCollectorDbContext>();
                    optionsBuilder.UseNpgsql(configuration.GetConnectionString("mydb"));
                    services.AddSingleton<NewsCollectorDbContext>(s => new NewsCollectorDbContext(optionsBuilder.Options));

                    services.AddSingleton<IUnitOfWork, UnitOfWork>();
                    services.AddSingleton<ISourceService, SourceService>();
                    services.AddSingleton<IKeywordService, KeywordService>();
                    services.AddSingleton<INewsKeywordService, NewsKeywordService>();
                    services.AddSingleton<INewsService, NewsService>();

                    #region snippet1
                    services.AddHostedService<CollectNewsByKeywordsService>();
                    #endregion


                })
                .Build();

            await host.StartAsync();
            await host.WaitForShutdownAsync();
        }
    }
}
