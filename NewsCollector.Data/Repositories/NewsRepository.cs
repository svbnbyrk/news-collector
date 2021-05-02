using Microsoft.EntityFrameworkCore;
using NewsCollector.Core.Domain;
using NewsCollector.Core.Models;
using NewsCollector.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsCollector.Data.Repositories
{
    public class NewsRepository : Repository<News>, INewsRepository
    {
        public NewsRepository(NewsCollectorDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<News>> GetNewsBySourceId(int sourceId)
        {
            return await NewsCollectorDbContext.News
                .Where(z => z.SourceId == sourceId)
                .ToListAsync();
        }

        public async Task<IEnumerable<News>> GetNewsBySearchTermintheTitle(string searchTerm)
        {
            return await NewsCollectorDbContext.News
                .Where(c => c.NewsTitle.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<News> GetNewsByUrlWithNewsKeyword(string url)
        {
            return await NewsCollectorDbContext.News
                .Include(x => x.NewsKeywords)
                .Where(x => x.NewsUrl.Equals(url))
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<News>> GetNewsByKeywordId(int keywordId)
        {
            return await NewsCollectorDbContext.News
             .Where(x => x.NewsKeywords.Any(z => z.KeywordId == keywordId)).ToListAsync();
        }

<<<<<<< HEAD
        public async Task<IEnumerable<News>> GetAllAsync(PaginationFilter pagination = null)
        {
            if (pagination == null)
            {
                return await NewsCollectorDbContext.News.Include(x => x.Source).ToListAsync();
            }
            var skip = (pagination.PageNumber - 1) * pagination.PageSize;

            return await NewsCollectorDbContext.News.Include(x => x.Source).OrderByDescending(x => x.NewsDate)
            .Skip(skip).Take(pagination.PageSize).ToListAsync();
        }

=======
>>>>>>> origin/development
        private NewsCollectorDbContext NewsCollectorDbContext
        {
            get { return Context as NewsCollectorDbContext; }
        }
    }
}
