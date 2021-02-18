using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewsCollector.Core.Models;
using NewsCollector.Core.Services;
using NewsCollector.Data;

namespace NewsCollector.WorkerService.Services
{
    public class CollectNewsByKeywordsService : IHostedService, IDisposable
    {
        private readonly ISourceService _sourceService;
        private readonly IKeywordService _keywordService;
        private readonly INewsService _newsService;
        private readonly INewsKeywordService _newsKeywordService;
        private readonly ILogger _logger;
        private Timer _timer;
        private Task _executingTask;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

        public CollectNewsByKeywordsService(ILogger<CollectNewsByKeywordsService> logger, ISourceService sourceService, IKeywordService keywordService, INewsService newsService, INewsKeywordService newsKeywordService)
        {
            _logger = logger;
            _sourceService = sourceService;
            _keywordService = keywordService;
            _newsService = newsService;
            _newsKeywordService = newsKeywordService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var number = GetRandomNumber(15, 30);
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(ExecuteTask, null, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(-1));

            return Task.CompletedTask;
        }

        private void ExecuteTask(object state)
        {
            _timer?.Change(Timeout.Infinite, 0);
            _executingTask = ExecuteTaskAsync(_stoppingCts.Token);
        }

        private async Task ExecuteTaskAsync(CancellationToken stoppingToken)
        {
            var number = GetRandomNumber(15, 30);
            await RunJobAsync(stoppingToken);
            _timer.Change(TimeSpan.FromMinutes(number), TimeSpan.FromMilliseconds(-1));
        }

        /// <summary>
        /// Bu metod <see cref="IHostedService"/> çağırıldığında başlar. 
        /// </summary>
        /// <param name="stoppingToken"><see cref="IHostedService.StopAsync(CancellationToken)"/> çağırıldığı zaman aktif olacaktır </param>
        /// <returns>A <see cref="Task"/>Uzun soluklu işlemler içermesi uygundur </returns>
        protected Task RunJobAsync(CancellationToken stoppingToken)
        {
            return TaskAsync();
        }

        private async Task TaskAsync()
        {
            try
            {
                var keywords = await _keywordService.GetAllKeywords();
                string link = "";
                _logger.LogInformation("{0} Adet anahtar kelime bulundu", keywords.Count());

                foreach (var item in keywords)
                {
                    //dinamik link oluşturmak için tüm keywordleri foreach ile dolaşıyorum
                    link = $"https://news.google.com/rss/search?q=%7B{item.KeywordValue}%7D&hl=tr&gl=TR&ceid=TR:tr";

                    //oluşturduğumuz linkteki rss dosyasını xml parse edip döndüren fonksiyon
                    var xml = GetRssXml(link);

                    //google rss de haber içerikleri item elementlerinden oluşur xml içinden item node bilgilerini diziye alıyorum
                    XmlNodeList entries = xml.DocumentElement.GetElementsByTagName("item");

                    _logger.LogInformation("{0} anahtar kelimesi içeren {1} adet haber bulundu. ", item.KeywordValue, entries.Count);

                    foreach (XmlNode entry in entries)
                    {
                        //haber linki uniq değerimiz haber db de varsa kayıt işlemi yapmayıp sıradaki habere geçiyorum
                        var isExistNews = await _newsService.GetNewsByUrlWithNewsKeyword(entry["link"].InnerText);

                        if (isExistNews != null)
                        {
                            if (isExistNews.NewsKeywords.FirstOrDefault(x => x.KeywordId == item.Id) != null)
                                continue;

                            await _newsKeywordService.CreateNewsKeyword(new NewsKeyword
                            {
                                Keyword = item,
                                News = isExistNews
                            });
                            continue;
                        }

                        var title = entry["title"].InnerText;
                        var index = title.Split("-").Reverse().FirstOrDefault().Length;
                        var cleanString = title.Remove(title.Length - index - 1, index + 1);

                        var news = new News
                        {
                            NewsTitle = cleanString,
                            NewsDate = DateTime.Parse(entry["pubDate"].InnerText),
                            NewsUrl = entry["link"].InnerText,
                            //GetSource methodu ile ilgili kaynak db de mevcut değilse db ye kayıt edip News tablosuna haberle beraber yazılıyor
                            Source = await GetSource(entry)
                        };

                        await _newsService.CreateNews(news);
                        await _newsKeywordService.CreateNewsKeyword(new NewsKeyword
                        {
                            Keyword = item,
                            News = news
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Bir hatayla karşılaşıldı: {0}", ex.ToString());
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

        public XmlDocument GetRssXml(string link)
        {
            XmlDocument xml = new XmlDocument();
            Uri url = new Uri(link);
            WebClient client = new WebClient();
            string html = client.DownloadString(url);
            xml.LoadXml(html);
            return xml;
        }

        public string ClearHtmlTags(string node)
        {
            node = Regex.Replace(node, "<[^>]*>", string.Empty);
            return node;
        }

        public int GetRandomNumber(int a, int b)
        {
            Random rnd = new Random();
            int month = rnd.Next(a, b);  // creates a number
            return month;
        }

        public async Task<Source> GetSource(XmlNode entry)
        {
            var source = new Source();
            try
            {
                var dem = await _sourceService.GetSourceBySearchTermAsync(entry["source"].Attributes["url"].InnerText.Remove(0, 8));
                if (dem != null)
                    return dem;
                else
                {
                    return await _sourceService.CreateSource(new Source
                    {
                        SourceName = entry["source"].InnerText,
                        WebAdress = entry["source"].Attributes["url"].InnerText.Remove(0, 8),
                    });
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return source;
            }

        }
    }
}
