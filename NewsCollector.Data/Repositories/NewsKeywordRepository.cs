using Microsoft.EntityFrameworkCore;
using NewsCollector.Core.Models;
using NewsCollector.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NewsCollector.Data.Repositories
{
    public class NewsKeywordRepository : Repository<NewsKeyword>, INewsKeywordRepository
    {
        public NewsKeywordRepository(NewsCollectorDbContext context) : base(context)
        {
        }

        public async Task<NewsKeyword> GetNewsKeywordsbyKeywordId(int keywordId)
        {
            return await NewsCollectorDbContext.NewsKeywords
                .Include(x => x.Keyword)
                .SingleOrDefaultAsync(x => x.KeywordId == keywordId);
        }

        public async Task<IEnumerable<News>> GetNewsByKeywordValue(string keywordValue)
        {
            return await NewsCollectorDbContext.NewsKeywords
                .Include(x => x.News)
                .Where(c => c.Keyword.KeywordValue.Equals(keywordValue))
                .Select(z => new News()
                {
                    Id = z.News.Id,
                    NewsDate = z.News.NewsDate,
                    NewsTitle = z.News.NewsTitle,
                    NewsUrl = z.News.NewsUrl,
                    SourceId = z.News.SourceId
                }).ToListAsync();               
        }

        private NewsCollectorDbContext NewsCollectorDbContext => Context as NewsCollectorDbContext;


    }
}
