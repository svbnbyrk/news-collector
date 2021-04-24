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
    public class SourceController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISourceService _sourceService;
        private readonly INewsService _newsService;

        public SourceController(IMapper mapper, ISourceService sourceService, INewsService newsService)
        {
            _mapper = mapper;
            _sourceService = sourceService;
            _newsService = newsService;

        }
        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<SourceDTO>>> GetAll()
        {
            var allSource = await _sourceService.GetAllSources();
            if (allSource == null)
                return NotFound();

            var allSourceDto = _mapper.Map<IEnumerable<Source>, IEnumerable<SourceDTO>>(allSource);

            return Ok(allSourceDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var source = await _sourceService.GetSourceById(id);
            if (source == null)
                return NotFound();

            var sourceDto = _mapper.Map<Source, SourceDTO>(source);

            return Ok(new Response<SourceDTO>(sourceDto));
        }

        [HttpGet("{id}/News")]
        public async Task<IActionResult> GetAll(int id)
        {
            var news = await _newsService.GetNewsBySourceId(id);

            if (news == null)
                return NotFound();

            var sourceDto = _mapper.Map<IEnumerable<News>, IEnumerable<NewsDTO>>(news);

            return Ok(new Response<IEnumerable<NewsDTO>>(sourceDto));
        }

        [HttpGet("{sourceId}/News/{newsId}")]
        public async Task<IActionResult> Get(int sourceId, int newsId)
        {
            var source = await _sourceService.GetSourceById(sourceId);
            if (source == null)
                return NotFound();

            var news = source.News.FirstOrDefault(x => x.Id == newsId);
            if (news == null)
                return NotFound();

            var newsDto = _mapper.Map<News, NewsDTO>(news);

            return Ok(new Response<NewsDTO>(newsDto));
        }

        [HttpPost]
        public async Task<IActionResult> CreateSource([FromBody] AddSourceDTO addSourceDTO)
        {
            var sourceDto = _mapper.Map<AddSourceDTO, Source>(addSourceDTO);
            var addSource = await _sourceService.CreateSource(sourceDto);

            return Ok(new Response<Source>(addSource));
        }
    }
}
