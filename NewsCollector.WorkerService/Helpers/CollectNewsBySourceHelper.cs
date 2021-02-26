using Microsoft.Extensions.Logging;
using NewsCollector.Core.Models;
using NewsCollector.Core.Services;
using NewsCollector.WorkerService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static NewsCollector.WorkerService.Helpers.CollectNewsBySourceHelper;

namespace NewsCollector.WorkerService.Helpers
{
    public class CollectNewsBySourceHelper : ICollectNewsBySourceHelper
    {
        private readonly ILogger<TimedWorker> _logger;
        private readonly ISourceService _sourceService;
        private readonly IKeywordService _keywordService;
        private readonly INewsService _newsService;
        private readonly INewsKeywordService _newsKeywordService;

        public interface ICollectNewsBySourceHelper
        {
            Task CollectNewsBySourceAsync();
        }

        public CollectNewsBySourceHelper(ILogger<TimedWorker> logger, ISourceService sourceService, IKeywordService keywordService, INewsService newsService, INewsKeywordService newsKeywordService)
        {
            _logger = logger;
            _sourceService = sourceService;
            _keywordService = keywordService;
            _newsService = newsService;
            _newsKeywordService = newsKeywordService;
        }

        public async Task CollectNewsBySourceAsync()
        {
            BaseHelper baseHelper = new BaseHelper();
            var sources = await _sourceService.GetAllSources();
            var link = "";
            foreach (var source in sources)
            {
                link = $"https://news.google.com/search?q=%20site%3A{source.WebAdress}&hl=tr&gl=TR&ceid=TR%3Atr";

                var xml = baseHelper.GetRssXml(link);

                //google rss de haber içerikleri item elementlerinden oluşur xml içinden item node bilgilerini diziye alıyorum
                XmlNodeList entries = xml.DocumentElement.GetElementsByTagName("item");

                foreach (XmlNode entry in entries)
                {
                    var isExistNews = await _newsService.GetNewsByUrlWithNewsKeyword(entry["link"].InnerText);

                    if (isExistNews != null)
                        continue;

                    var title = entry["title"].InnerText;
                    var index = title.Split("-").Reverse().FirstOrDefault().Length;
                    var cleanString = title.Remove(title.Length - index - 1, index + 1);

                    var news = new News
                    {
                        NewsTitle = cleanString,
                        NewsDate = DateTime.Parse(entry["pubDate"].InnerText),
                        NewsUrl = entry["link"].InnerText,
                        //GetSource methodu ile ilgili kaynak db de mevcut değilse db ye kayıt edip News tablosuna haberle beraber yazılıyor
                        Source = source
                    };

                }
            }

            _logger.LogInformation("CollectNewsBySourceAsync metodu çalıştı.");
        }
    }
}
