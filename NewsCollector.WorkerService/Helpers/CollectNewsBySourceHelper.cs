using Microsoft.Extensions.Logging;
using NewsCollector.Core.Models;
using NewsCollector.Core.Services;
using NewsCollector.WorkerService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static NewsCollector.WorkerService.Helpers.CollectNewsBySourceHelper;

namespace NewsCollector.WorkerService.Helpers
{
    public class CollectNewsBySourceHelper : ICollectNewsBySourceHelper
    {
        private readonly ILogger<CollectNewsBySourceHelper> _logger;
        private readonly ISourceService _sourceService;
        private readonly INewsService _newsService;
        private readonly IHttpClientFactory _httpClientFactory;

        public interface ICollectNewsBySourceHelper
        {
            Task CollectNewsBySourceAsync();
        }

        public CollectNewsBySourceHelper(ILogger<CollectNewsBySourceHelper> logger, ISourceService sourceService, INewsService newsService, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _sourceService = sourceService;
            _newsService = newsService;
            _httpClientFactory = httpClientFactory;
        }

        public async Task CollectNewsBySourceAsync()
        {
            var sources = await _sourceService.GetAllSources();
            _logger.LogInformation("{0} Adet haber kaynağı bulundu", sources.Count());
            var link = "";
            foreach (var source in sources)
            {
                int newNews = 0;
                link = $"https://news.google.com/rss/search?q=%20site%3A{source.WebAdress}&hl=tr&gl=TR&ceid=TR%3Atr";

                XmlDocument xml = new XmlDocument();
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(link);
                string result = await client.GetStringAsync("");
                xml.LoadXml(result);

                //google rss de haber içerikleri item elementlerinden oluşur xml içinden item node bilgilerini diziye alıyorum
                XmlNodeList entries = xml.DocumentElement.GetElementsByTagName("item");

                foreach (XmlNode entry in entries)
                {
                    try
                    {
                        var isExistNews = await _newsService.GetNewsByUrlWithNewsKeyword(entry["link"].InnerText);

                        if (isExistNews != null)
                            continue;

                        newNews++;
                        var title = entry["title"].InnerText;
                        var index = title.Split("-").Reverse().FirstOrDefault().Length;
                        var cleanString = title.Remove(title.Length - index - 1, index + 1);

                        var news = new News
                        {
                            NewsTitle = cleanString,
                            NewsDate = DateTime.Parse(entry["pubDate"].InnerText),
                            NewsUrl = entry["link"].InnerText,
                            Source = source
                        };

                        await _newsService.CreateNews(news);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                        continue;
                    }
                }
                _logger.LogInformation("{0} Haber kaynağına ait {1} adet yeni haber bulundu.", source.SourceName, newNews);
            }
        }
    }
}

