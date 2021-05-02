using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsCollector.Core.Domain.Responses;
using NewsCollector.Core.Models;
using NewsCollector.Core.Services;
using NewsCollector.DTO;

namespace NewsCollector.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeywordController : ControllerBase
    {
        private readonly IKeywordService keywordService;
        private readonly IMapper _mapper;
        private readonly INewsService newsService;
        private readonly ISourceService sourceService;

        public KeywordController(IKeywordService keywordService, IMapper mapper, INewsService newsService,ISourceService sourceService)
        {
            _mapper = mapper;
            this.keywordService = keywordService;
            this.newsService = newsService;
            this.sourceService = sourceService;
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateKeyword([FromBody] AddKeywordDTO addKeywordResource)
        {
            Keyword keywordModel = _mapper.Map<AddKeywordDTO, Keyword>(addKeywordResource);
            var newKeyword = await keywordService.CreateKeyword(keywordModel);
            var keyword = await keywordService.GetKeywordById(newKeyword.Id);
            var artistModel = _mapper.Map<Keyword, KeywordDTO>(keyword);

            return Ok(new Response<KeywordDTO>(artistModel));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var keywords = await keywordService.GetAllKeywords();
            if (keywords == null)
                return NotFound();

            var keywordDTO = _mapper.Map<IEnumerable<Keyword>, IEnumerable<KeywordDTO>>(keywords);
            return Ok(new Response<IEnumerable<KeywordDTO>>(keywordDTO));
        }

        [HttpGet("{id}/News")]
        public async Task<IActionResult> GetNewsByKeyword(int id)
        {
            var news = await newsService.GetNewsByKeywordId(id);
            if (news == null)
                return NotFound();

            var newsDto = _mapper.Map<IEnumerable<News>, IEnumerable<NewsDTO>>(news);
            return Ok(new Response<IEnumerable<NewsDTO>>(newsDto));
        }


        [HttpGet("{id}/news-count-by-source")]
        public async Task<IActionResult> GetNewsByKeywordWithSource(int id)
        {
            var news = await newsService.GetNewsByKeywordId(id);
            if (news == null)
                return NotFound();

            var count = news.GroupBy(x => x.SourceId)
            .Select(c => new GraphDTO
            {
                Id = keywordService.GetKeywordById(id).Result.KeywordValue,
                Key = sourceService.GetSourceById(c.Key).Result.SourceName,
                Value = c.Count().ToString()
            });

            return Ok(new Response<IEnumerable<GraphDTO>>(count));
        }

    }
}
