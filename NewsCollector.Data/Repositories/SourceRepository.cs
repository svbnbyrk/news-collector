using Microsoft.EntityFrameworkCore;
using NewsCollector.Core.Models;
using NewsCollector.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace NewsCollector.Data.Repositories
{
    public class SourceRepository : Repository<Source>, ISourceRepository
    {
        public SourceRepository(NewsCollectorDbContext context) : base(context)
        { }

        public async Task<Source> GetSourceBySearchTermAsync(string searchTerm)
        {
            return await NewsCollectorDbContext.Sources
                .FirstOrDefaultAsync(x => x.WebAdress == searchTerm);            
        }

        private NewsCollectorDbContext NewsCollectorDbContext
        {
            get { return Context as NewsCollectorDbContext; }
        }
    }
}
