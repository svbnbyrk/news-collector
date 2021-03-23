using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsCollector.Core.Models;
using NewsCollector.Core.Services;
using NewsCollector.DTO;

namespace NewsCollector.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeywordController : ControllerBase
    {
        private readonly IKeywordService _keywordService;
        private readonly IMapper _mapper;
        private readonly INewsService newsService;

        public KeywordController(IKeywordService keywordService, IMapper mapper, INewsService newsService)
        {
            _mapper = mapper;
            _keywordService = keywordService;
            this.newsService = newsService;
        }

        [HttpPost("")]
        public async Task<ActionResult<KeywordDTO>> CreateKeyword([FromBody] AddKeywordDTO addKeywordResource)
        {
            Keyword keywordModel = _mapper.Map<AddKeywordDTO, Keyword>(addKeywordResource);
            var newKeyword = await _keywordService.CreateKeyword(keywordModel);
            var keyword = await _keywordService.GetKeywordById(newKeyword.Id);
            var artistModel = _mapper.Map<Keyword, KeywordDTO>(keyword);

            return Ok(artistModel);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<KeywordDTO>>> GetAll()
        {
            var keywords = await _keywordService.GetAllKeywords();
            if (keywords == null)
                return NotFound();

            var keywordDTO = _mapper.Map<IEnumerable<Keyword>, IEnumerable<KeywordDTO>>(keywords);
            return Ok(keywordDTO);
        }

        [HttpGet("{id}/News")]
        public async Task<ActionResult<IEnumerable<NewsDTO>>> GetNewsByKeyword(int id)
        {
            var news = await newsService.GetNewsByKeywordId(id);
            if(news == null)
                return NotFound();

            var newsDto = _mapper.Map<IEnumerable<News>, IEnumerable<NewsDTO>>(news);
            return Ok(newsDto);           
        }

        // [HttpPut]
        // public async Task<ActionResult<KeywordDTO>> UpdateKeyword(int Id, [FromBody] KeywordDTO updateKeywordResource)
        // {
        //     var keywordDTO = _mapper.Map<KeywordDTO, Keyword>(updateKeywordResource);
        //     if (Id != updateKeywordResource.Id)
        //         return BadRequest();        
            
        //     var keyword = await _keywordService.GetKeywordById(Id);

        //     if (keyword == null)
        //         return NotFound();

        //     await _keywordService.UpdateKeyword(keywordDTO,keyword);

        //     return Ok();
        // }
    }
}
