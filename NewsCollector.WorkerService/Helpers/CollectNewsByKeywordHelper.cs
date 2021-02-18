//using Microsoft.Extensions.Logging;
//using NewsCollector.Core.Models;
//using NewsCollector.Core.Services;
//using NewsCollector.Data;
//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using System.Xml;

//namespace NewsCollector.WorkerService.Helpers
//{
//    public class CollectNewsByKeywordHelper
//    {
//        private readonly ILogger<Worker> _logger;
//        private readonly ISourceService _sourceService;
//        private readonly IKeywordService _keywordService;
//        private readonly INewsService _newsService;
//        private readonly INewsKeywordService _newsKeywordService;
//        private readonly NewsCollectorDbContext _newsCollectorDbContext;
//        public CollectNewsByKeywordHelper(ILogger<Worker> logger, ISourceService sourceService, IKeywordService keywordService, INewsService newsService, INewsKeywordService newsKeywordService)
//        {
//            _logger = logger;
//            _sourceService = sourceService;
//            _keywordService = keywordService;
//            _newsService = newsService;
//            _newsKeywordService = newsKeywordService;
//        }
//    }
//}
