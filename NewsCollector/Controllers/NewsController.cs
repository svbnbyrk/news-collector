using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsCollector.Core.Services;
using NewsCollector.Data;
using NewsCollector.DTO;

namespace NewsCollector.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;
        private readonly IMapper _mapper;
        private readonly NewsCollectorDbContext _dbContext;

        public NewsController(INewsService newsService, IMapper mapper, NewsCollectorDbContext dbContext)
        {
            this._mapper = mapper;
            this._newsService = newsService;
            this._dbContext = dbContext;
        }

        [HttpGet("")]
        public ActionResult<IEnumerable<TopFiveKeywordCountDTO>> GetTopFiveKeywordCount()
        {
            var topFiveKeywordCounts = _dbContext.NewsKeywords
                .GroupBy(x => x.Keyword.KeywordValue)
                .Select(z => new TopFiveKeywordCountDTO
                {
                    KeywordName = z.Key,
                    Count = z.Count()
                });

            return Ok(topFiveKeywordCounts);
        }
    }
}
