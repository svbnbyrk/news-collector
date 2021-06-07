using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsCollector.Core.Domain.Responses;
using NewsCollector.Core.Models;
using NewsCollector.Data;
using NewsCollector.DTO;

namespace NewsCollector.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class GraphsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly NewsCollectorDbContext _dbContext;

        public GraphsController(IMapper mapper, NewsCollectorDbContext dbContext)
        {
            this._mapper = mapper;
            this._dbContext = dbContext;
        }

        [HttpGet("news-count-by-keyword")]
        public ActionResult<IEnumerable<GraphDTO>> KeywordCount([FromQuery] SearchByDateDTO searchByDate)
        {
            DateTime startDate;
            DateTime endDate;
            var query = _dbContext.NewsKeywords.AsQueryable();
            if (searchByDate.StartingDate != null || searchByDate.StartingDate != null)
            {
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

                query = query.Where(c => c.News.NewsDate >= startDate && c.News.NewsDate < endDate);
            }

            var topKeywordCounts = query
                .GroupBy(x => x.Keyword.KeywordValue)
                .Select(z => new GraphDTO
                {
                    Id = _dbContext.Keywords.FirstOrDefault(x => x.KeywordValue.Contains(z.Key)).Id.ToString(),
                    Key = z.Key,
                    Value = z.Count().ToString()
                })
                .ToList();

            if (topKeywordCounts?.Count == 0)
                return NotFound();

            return Ok(new Response<IEnumerable<GraphDTO>>(topKeywordCounts));
        }


        //TO-DO daha sonrasında bu fonksiyon db view yapılabilir
        [HttpGet("news-count-by-source")]
        public ActionResult<IEnumerable<GraphDTO>> NewsCountBySource([FromQuery] SearchByDateDTO searchByDate)
        {
            DateTime startDate;
            DateTime endDate;
            var query = _dbContext.News.AsQueryable();
            if (searchByDate.StartingDate != null || searchByDate.StartingDate != null)
            {
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

                query = query.Where(c => c.NewsDate >= startDate && c.NewsDate < endDate);
            }

            var getNewsCountBySource = query.Where(x => x.NewsKeywords.Count > 0)
            .GroupBy(x => new { x.SourceId })
            .Select(c => new GraphDTO
            {
                Id = c.Key.SourceId.ToString(),
                Key = _dbContext.Sources.Where(x => x.Id == c.Key.SourceId).Select(x => x.SourceName).FirstOrDefault(),
                Value = c.Count().ToString()
            })
            .OrderByDescending(x => x.Value)
            .Take(20)
            .ToList();

            if (getNewsCountBySource?.Count == 0)
                return NotFound();

            return Ok(getNewsCountBySource);
        }

        [HttpGet("news-count")]
        public ActionResult<IEnumerable<CountDTO>> NewsCount([FromQuery] SearchByDateDTO searchByDate)
        {
            var query = _dbContext.News.Where(x => x.NewsKeywords.Count > 0).AsQueryable();
            if (searchByDate.EndingDate != null && searchByDate.StartingDate != null)
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

                query = query.Where(c => c.NewsDate >= startDate && c.NewsDate < endDate);
            }

            var newsCount = query.Count();
            if (newsCount == 0)
                return NotFound();

            return Ok(new Response<int>(newsCount));
        }

        [HttpGet("source-count")]
        public ActionResult<IEnumerable<CountDTO>> SourceCount([FromQuery] SearchByDateDTO searchByDate)
        {
            if (searchByDate.EndingDate != null && searchByDate.StartingDate != null)
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
            }

            var sourceCount = _dbContext.Sources
            .Count();

            if (sourceCount == 0)
                return NotFound();

            return Ok(sourceCount);
        }

        [HttpGet("news-count-by-source/weekly")]
        public ActionResult<IEnumerable<GraphDTO>> NewsCountBySource(int sourceId)
        {
            var now = DateTime.Now.Date;

            IQueryable<News> query;
            List<GraphDTO> getNewsCountBySource = new List<GraphDTO>();

            for (int i = 0; i < 7; i++)
            {
                query = _dbContext.News
               .Where(c => c.NewsDate >= now.AddDays(-i) && c.NewsDate < now.AddDays(-(i - 1)));

                if (sourceId > 0)
                {
                    query = query.Where(x => x.SourceId == sourceId);
                }

                var dem = query.GroupBy(x => new { x.SourceId }).Count();

                getNewsCountBySource.Add(new GraphDTO
                {
                    Id = sourceId.ToString(),
                    Key = now.AddDays(-i).ToShortDateString(),
                    Value = dem.ToString()
                });
            }
            return Ok(getNewsCountBySource);
        }

        [HttpGet("news-count-by-keyword/weekly")]
        public ActionResult<IEnumerable<GraphDTO>> NewsCountByKeyword([FromQuery] int keywordId)
        {
            var now = DateTime.Now.Date;
            var query = _dbContext.NewsKeywords.AsQueryable();
            List<GraphDTO> getNewsCountByKeyword = new List<GraphDTO>();

            for (int i = 0; i < 7; i++)
            {
                var dem = query.Where(c => c.News.NewsDate >= now.AddDays(-i) && c.News.NewsDate < now.AddDays(-(i - 1)))
                .Where(x => x.KeywordId == keywordId)
                .GroupBy(x => new { x.Keyword.KeywordValue })
                .Count();

                getNewsCountByKeyword.Add(new GraphDTO
                {
                    Id = keywordId.ToString(),
                    Key = now.AddDays(-i).ToShortDateString(),
                    Value = dem.ToString()
                });
            }
            return Ok(getNewsCountByKeyword);
        }
    }
}
