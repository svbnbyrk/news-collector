using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsCollector.Core.Models;
using NewsCollector.Core.Services;
using NewsCollector.Data;
using NewsCollector.DTO;
using NewsCollector.Helpers;
using NewsCollector.Domain.Responses;
using NewsCollector.Domain.Queries;
using NewsCollector.Domain;
using NewsCollector.Core.Domain.Queries;
using NewsCollector.Core.Domain;
using NewsCollector.Core.Domain.Responses;

namespace NewsCollector.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;
        private readonly ISourceService _sourceService;
        private readonly IMapper _mapper;
        private readonly IKeywordService _keywordService;
        private readonly IHttpClientFactory _httpClientFactory;

        public NewsController(INewsService newsService, IMapper mapper, ISourceService sourceService, IKeywordService keywordService, IHttpClientFactory httpClientFactory)
        {
            this._mapper = mapper;
            this._newsService = newsService;
            this._sourceService = sourceService;
            this._keywordService = keywordService;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery] string searchTerm)
        {
            var link = $"https://news.google.com/rss/search?q=%7B{searchTerm}%7D&hl=tr&gl=TR&ceid=TR:tr";
            var newsList = new List<NewsDTO>();

            XmlDocument xml = new XmlDocument();
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(link);
            string result = await client.GetStringAsync("");
            xml.LoadXml(result);
            XmlNodeList entries = xml.DocumentElement.GetElementsByTagName("item");

            if (entries.Count == 0)
                return NotFound();

            foreach (XmlNode entry in entries)
            {
                var random = new Random();
                var title = entry["title"].InnerText;
                var index = title.Split("-").Reverse().FirstOrDefault().Length;
                var cleanString = title.Remove(title.Length - index - 1, index + 1);
                var news = new NewsDTO
                {
                    Id = random.Next(1000, 3000),
                    NewsTitle = cleanString,
                    NewsDate = DateTime.Parse(entry["pubDate"].InnerText),
                    NewsUrl = entry["link"].InnerText,
                    SourceName = entry["source"].InnerText
                };
                newsList.Add(news);
            }
            return Ok(newsList);
        }

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery] PaginationQuery pagination)
        {
            var paginationFilter = _mapper.Map<PaginationFilter>(pagination);
            var getallnews = await _newsService.GetAllNews(paginationFilter);
            var paginationResponse = new PagedResponse<NewsDTO>();
            if (getallnews == null)
                return NotFound();


            var getallnewsDto = _mapper.Map<IEnumerable<News>, IEnumerable<NewsDTO>>(getallnews);

            foreach (var item in getallnewsDto)
            {
                var source = await _sourceService.GetSourceById(item.SourceId);
                item.SourceName = source.SourceName;
            }

            return Ok(new PagedResponse<NewsDTO>(getallnewsDto));

        }

        [Authorize]
        [HttpGet("id")]
        public async Task<ActionResult<NewsDTO>> Get(int id)
        {
            var getNews = await _newsService.GetNewsById(id);
            if (getNews == null)
                return NotFound();

            var newsDto = _mapper.Map<News, NewsDTO>(getNews);
            return newsDto;
        }

        [HttpGet("{id}/Source")]
        public async Task<ActionResult<SourceDTO>> GetSource(int id)
        {
            var getNews = await _newsService.GetNewsById(id);
            if (getNews == null)
                return NotFound();

            var getSource = await _sourceService.GetSourceById(getNews.SourceId);
            if (getSource == null)
                return NotFound();

            var sourceDTO = _mapper.Map<Source, SourceDTO>(getSource);
            return sourceDTO;
        }
    }
}
