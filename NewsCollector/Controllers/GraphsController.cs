using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
using Microsoft.EntityFrameworkCore;
using NewsCollector.Core.Domain.Responses;
=======
>>>>>>> origin/development
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
<<<<<<< HEAD
        public IActionResult KeywordCount()
=======
        public ActionResult<IEnumerable<GraphDTO>> KeywordCount()
>>>>>>> origin/development
        {
            var topKeywordCounts = _dbContext.NewsKeywords
                .GroupBy(x => x.Keyword.KeywordValue)
                .Select(z => new GraphDTO
                {
<<<<<<< HEAD
                    Id = _dbContext.Keywords.FirstOrDefault(x => x.KeywordValue.Contains(z.Key)).Id.ToString(),
=======
                    Id = _dbContext.Keywords.FirstOrDefault(x => x.KeywordValue.Contains(z.Key)).Id,
>>>>>>> origin/development
                    Key = z.Key,
                    Value = z.Count()
                })
                .ToList();

            if (topKeywordCounts?.Count == 0)
                return NotFound();

            return Ok(new Response<IEnumerable<GraphDTO>>(topKeywordCounts));
        }

        [HttpPost("news-count-by-keyword")]
<<<<<<< HEAD
        public IActionResult KeywordCount([FromQuery] SearchByDateDTO searchByDate)
=======
        public ActionResult<IEnumerable<GraphDTO>> KeywordCount([FromBody] SearchByDateDTO searchByDate)
>>>>>>> origin/development
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
                .Select(z => new GraphDTO
                {
<<<<<<< HEAD
                    Id = _dbContext.Keywords.FirstOrDefault(x => x.KeywordValue.Contains(z.Key)).Id.ToString(),
=======
                    Id = _dbContext.Keywords.FirstOrDefault(x => x.KeywordValue.Contains(z.Key)).Id,
>>>>>>> origin/development
                    Key = z.Key,
                    Value = z.Count()
                })
                .ToList();

            if (topKeywordCounts?.Count == 0)
                return NotFound();

            return Ok(new Response<IEnumerable<GraphDTO>>(topKeywordCounts));
        }


        [HttpGet("news-count-by-source")]
        public IActionResult NewsCountBySource()
        {
            var getNewsCountBySource = _dbContext.News
                .Where(x => x.NewsKeywords.FirstOrDefault() != null)
                .Select(z => new {
                    z.Id,
                    z.SourceId,
                    z.Source.SourceName
                })
                .GroupBy(y => y.SourceId)
                .Select(c => new GraphDTO
                {
<<<<<<< HEAD
                    Id = c.Key.ToString(),
                    Key = c.First().SourceName.ToString(),
=======
                    Id = c.Key.SourceId,
                    Key = _dbContext.Sources.Where(x => x.Id == c.Key.SourceId).Select(x => x.SourceName).FirstOrDefault(),
>>>>>>> origin/development
                    Value = c.Count()
                })
                .OrderByDescending(x => x.Value)
                .Take(20)
                .ToList();
<<<<<<< HEAD

            if (getNewsCountBySource?.Count == 0)
                return NotFound();

            var rand = new Random();
            var randomList = getNewsCountBySource.OrderBy(x => rand.Next()).ToList();

            return Ok(new Response<IEnumerable<GraphDTO>>(randomList));
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
=======

            if (getNewsCountBySource?.Count == 0)
>>>>>>> origin/development
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

                var dem = query.GroupBy(x => new { x.SourceId })
                .Select(c => new GraphDTO
                {
                    Id = sourceId.ToString(),
                    Key = now.AddDays(-i).ToShortDateString(),
                    Value = c.Count()
                }).ToList();

                getNewsCountBySource.AddRange(dem);
            }
            return Ok(getNewsCountBySource);
        }

<<<<<<< HEAD
=======
        [HttpGet("news-count")]
        public ActionResult<IEnumerable<CountDTO>> NewsCount()
        {
            var newsCount = _dbContext.News
            .Count();

            if (newsCount == 0)
                return NotFound();

            return Ok(newsCount);
        }

        [HttpGet("source-count")]
        public ActionResult<IEnumerable<CountDTO>> SourceCount()
        {
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

                var dem = query.GroupBy(x => new { x.SourceId })
                .Select(c => new GraphDTO
                {
                    Id = sourceId,
                    Key = now.AddDays(-i).ToShortDateString(),
                    Value = c.Count()
                }).ToList();

                getNewsCountBySource.AddRange(dem);
            }
            return Ok(getNewsCountBySource);
        }

>>>>>>> origin/development
        [HttpGet("news-count-by-keyword/weekly")]
        public ActionResult<IEnumerable<GraphDTO>> NewsCountByKeyword(int keywordId)
        {
            var now = DateTime.Now.Date;

            IQueryable<NewsKeyword> query;
            List<GraphDTO> getNewsCountByKeyword = new List<GraphDTO>();

            for (int i = 0; i < 7; i++)
            {
                query = _dbContext.NewsKeywords
               .Where(c => c.News.NewsDate >= now.AddDays(-i) && c.News.NewsDate < now.AddDays(-(i - 1)));

                if (keywordId > 0)
                {
                    query = query.Where(x => x.KeywordId == keywordId);
                }

                var dem = query.GroupBy(x => new { x.Keyword.KeywordValue })
                .Select(c => new GraphDTO
                {
<<<<<<< HEAD
                    Id = keywordId.ToString(),
=======
                    Id = keywordId,
>>>>>>> origin/development
                    Key = now.AddDays(-i).ToShortDateString(),
                    Value = c.Count()
                }).ToList();

                getNewsCountByKeyword.AddRange(dem);
            }
            return Ok(getNewsCountByKeyword);
        }
    }
}
