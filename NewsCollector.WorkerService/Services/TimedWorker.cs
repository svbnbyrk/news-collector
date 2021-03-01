using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewsCollector.Core.Models;
using NewsCollector.Core.Services;
using NewsCollector.Data;
using NewsCollector.WorkerService.Helpers;
using static NewsCollector.WorkerService.Helpers.CollectNewsByKeywordHelper;
using static NewsCollector.WorkerService.Helpers.CollectNewsBySourceHelper;

namespace NewsCollector.WorkerService.Services
{
    public class TimedWorker : IHostedService, IDisposable
    {
        public IServiceProvider Services { get; }
        private readonly ILogger _logger;
        private Timer _timer;
        private Task _executingTask;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
        private int number;

        public TimedWorker(ILogger<TimedWorker> logger, IServiceProvider services)
        {
            _logger = logger;
            Services = services;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            _timer = new Timer(ExecuteTask, null, TimeSpan.FromSeconds(2), TimeSpan.FromMilliseconds(-1));

            return Task.CompletedTask;
        }

        private void ExecuteTask(object state)
        {
            _timer?.Change(Timeout.Infinite, 0);
            _executingTask = ExecuteTaskAsync(_stoppingCts.Token);
        }

        private async Task ExecuteTaskAsync(CancellationToken stoppingToken)
        {
           number = GetRandomNumber(15, 30);
            await RunJobAsync(stoppingToken);
            _timer.Change(TimeSpan.FromMinutes(number), TimeSpan.FromMilliseconds(-1));
        }

        /// <summary>
        /// Bu metod async <see cref="Task"/> olmalı.
        /// </summary>
        /// <param name="stoppingToken"><see cref="IHostedService.StopAsync(CancellationToken)"/></param>
        /// <returns> Periyodik zamanlarda gerçekleşmesi uygun async <see cref="Task"/> değeri döndürür. </returns>
        protected async Task RunJobAsync(CancellationToken stoppingToken)
        {
            using (var scope = Services.CreateScope())
            {
                var collectNewsByKeywordHelper =
                    scope.ServiceProvider
                        .GetRequiredService<ICollectNewsByKeywordHelper>();

                await collectNewsByKeywordHelper.CollectNewsByKeywordAsync();
            }

            using (var scope = Services.CreateScope())
            {

                var collectNewsBySourceHelper =
                    scope.ServiceProvider
                        .GetRequiredService<ICollectNewsBySourceHelper>();

                await collectNewsBySourceHelper.CollectNewsBySourceAsync();
            }
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);

            if (_executingTask == null)
            {
                return;
            }

            try
            {
                _stoppingCts.Cancel();
            }
            finally
            {
                //Bütün işler bitene kadar bekler
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }

        }

        public void Dispose()
        {
            _stoppingCts.Cancel();
            _timer?.Dispose();
        }

        public int GetRandomNumber(int a, int b)
        {
            Random rnd = new Random();
            int month = rnd.Next(a, b);  // creates a number
            return month;
        }

    }
}
