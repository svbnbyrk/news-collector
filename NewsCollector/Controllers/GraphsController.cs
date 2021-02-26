using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsCollector.Data;
using NewsCollector.DTO;

namespace NewsCollector.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GraphsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly NewsCollectorDbContext _dbContext;

        public GraphsController(IMapper mapper, NewsCollectorDbContext dbContext)
        {
            this._mapper = mapper;
            this._dbContext = dbContext;
        }

        [HttpGet("keyword-count")]
        public ActionResult<IEnumerable<TopFiveKeywordCountDTO>> KeywordCount()
        {
            var topKeywordCounts = _dbContext.NewsKeywords
                .GroupBy(x => x.Keyword.KeywordValue)
                .Select(z => new TopFiveKeywordCountDTO
                {
                    KeywordName = z.Key,
                    Count = z.Count()
                }).ToList();

            if (topKeywordCounts == null)
                return NotFound();

            return Ok(topKeywordCounts);
        }

        [HttpPost("keyword-count")]
        public ActionResult<IEnumerable<TopFiveKeywordCountDTO>> KeywordCount([FromBody] SearchByDateDTO searchByDate)
        {
            DateTime startDate;
            DateTime endDate;
            if (!DateTime.TryParse(searchByDate.StartingDate, out startDate))
            {
                return BadRequest("StartingDate tarih formatında değil");
            }
            else if (!DateTime.TryParse(searchByDate.EndingDate, out endDate))
            {
                return BadRequest("EndingDate tarih formatında değil");
            }

            if (startDate >= endDate)
            {
                return BadRequest("StartingDate, EndingDate tarihinden ileri bir tarih olamaz");
            }

            var topKeywordCounts = _dbContext.NewsKeywords
                .Where(c => c.News.NewsDate >= startDate && c.News.NewsDate < endDate)
                .GroupBy(x => x.Keyword.KeywordValue)
                .Select(z => new TopFiveKeywordCountDTO
                {
                    KeywordName = z.Key,
                    Count = z.Count()
                }).ToList();

            if (topKeywordCounts == null)
                return NotFound();

            return Ok(topKeywordCounts);
        }


        [HttpGet("news-count-by-source")]
        public ActionResult<IEnumerable<TopFiveKeywordCountDTO>> NewsCountBySource()
        {
            var getNewsCountBySource = _dbContext.News
                .GroupBy(x => new { x.SourceId })
                .Select(c => new TopFiveKeywordCountDTO
                {
                    KeywordName = _dbContext.Sources.Where(x => x.Id == c.Key.SourceId).Select(x => x.SourceName).FirstOrDefault(),
                    Count = c.Count()
                }).ToList();

            if (getNewsCountBySource == null)
                return NotFound();

            return Ok(getNewsCountBySource);
        }      
    }
}
