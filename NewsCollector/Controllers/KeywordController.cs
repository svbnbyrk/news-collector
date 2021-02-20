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

        public KeywordController(IKeywordService keywordService, IMapper mapper)
        {
            _mapper = mapper;
            _keywordService = keywordService;
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
    }
}
