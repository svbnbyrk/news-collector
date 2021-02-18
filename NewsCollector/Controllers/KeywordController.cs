using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsCollector.Core.Services;

namespace NewsCollector.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeywordController : ControllerBase
    {
        private readonly IKeywordService _newsService;
        private readonly IMapper _mapper;

        public KeywordController(IKeywordService keywordService, IMapper mapper)
        {

        }
    }
}
