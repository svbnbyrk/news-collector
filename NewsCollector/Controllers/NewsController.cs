using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsCollector.Core.Models;
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
        private readonly ISourceService _sourceService;
        private readonly IMapper _mapper;

        public NewsController(INewsService newsService, IMapper mapper, ISourceService sourceService)
        {
            this._mapper = mapper;
            this._newsService = newsService;
            this._sourceService = sourceService;
        }

        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<NewsDTO>>> GetAll()
        {
            var getallnews = await _newsService.GetAllNews();
            if (getallnews == null)
                return NotFound();

            var getallnewsDto = _mapper.Map<IEnumerable<News>, IEnumerable<NewsDTO>>(getallnews);
            return Ok(getallnewsDto);
        }

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

            var sourceDTO = _mapper.Map<Source, SourceDTO>(getSource);
            return sourceDTO;
        }
    }
}
