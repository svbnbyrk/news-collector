using Microsoft.EntityFrameworkCore;
using NewsCollector.Core.Models;
using NewsCollector.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsCollector.Data.Repositories
{
    public class KeywordRepository : Repository<Keyword>, IKeywordRepository
    {
        public KeywordRepository(NewsCollectorDbContext context) : base(context)
        {
        }

        public Task<Keyword> GetWithNewsKeywordByIdAsync(int id)
        {
            return NewsCollectorDbContext.Keywords
                .Include(a => a.NewsKeywords)
                .SingleOrDefaultAsync(a => a.Id == id );
        }

        private NewsCollectorDbContext NewsCollectorDbContext
        {
            get { return Context as NewsCollectorDbContext; }
        }
    }
}
