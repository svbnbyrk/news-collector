using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using NewsCollector.Core.Models;
using NewsCollector.Core.Services;
using NewsCollector.Data;
using NewsCollector.WorkerService.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using static NewsCollector.WorkerService.Helpers.CollectNewsByKeywordHelper;

namespace NewsCollector.WorkerService.Helpers
{
    public class CollectNewsByKeywordHelper : ICollectNewsByKeywordHelper
    {
        private readonly ILogger<TimedWorker> _logger;
        private readonly ISourceService _sourceService;
        private readonly IKeywordService _keywordService;
        private readonly INewsService _newsService;
        private readonly INewsKeywordService _newsKeywordService;
        private readonly IHttpClientFactory _httpClientFactory;
        public interface ICollectNewsByKeywordHelper
        {
            Task CollectNewsByKeywordAsync();
        }

        public CollectNewsByKeywordHelper(ILogger<TimedWorker> logger, ISourceService sourceService, IKeywordService keywordService, INewsService newsService, INewsKeywordService newsKeywordService, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _sourceService = sourceService;
            _keywordService = keywordService;
            _newsService = newsService;
            _newsKeywordService = newsKeywordService;
            _httpClientFactory = httpClientFactory;
        }

        public async Task CollectNewsByKeywordAsync()
        {
            try
            {
                BaseHelper baseHelper = new BaseHelper();
                var keywords = await _keywordService.GetAllKeywords();
                string link = "";
                _logger.LogInformation("{0} Adet anahtar kelime bulundu", keywords.Count());

                foreach (var item in keywords)
                {
                    int newNews = 0;
                    //dinamik link oluşturmak için tüm keywordleri foreach ile dolaşıyorum
                    link = $"https://news.google.com/rss/search?q=%7B{item.KeywordValue}%7D&hl=tr&gl=TR&ceid=TR:tr";

                    //oluşturduğumuz linkteki rss dosyasını xml parse edip döndüren fonksiyon
                    XmlDocument xml = new XmlDocument();
                    var client = _httpClientFactory.CreateClient();
                    client.BaseAddress = new Uri(link);
                    string result = await client.GetStringAsync("");
                    xml.LoadXml(result);

                    //google rss de haber içerikleri item elementlerinden oluşur xml içinden item node bilgilerini diziye alıyorum
                    XmlNodeList entries = xml.DocumentElement.GetElementsByTagName("item");

                    // _logger.LogInformation("{0} anahtar kelimesi içeren {1} adet haber bulundu. ", item.KeywordValue, entries.Count);

                    foreach (XmlNode entry in entries)
                    {
                        try
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
                            newNews++;
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
                        catch
                        {
                            continue;
                        }
                    }
                    _logger.LogInformation("{0} anahtar kelimesi içeren {1} adet yeni haber bulundu. ", item.KeywordValue, newNews);
                }
                _logger.LogInformation("CollectNewsByKeywordAsync metodu çalıştı");
            }
            catch (Exception ex)
            {

            }

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
