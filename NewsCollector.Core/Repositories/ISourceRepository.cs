using NewsCollector.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewsCollector.Core.Repositories
{
    public interface ISourceRepository : IRepository<Source>
    {
        Task<Source> GetSourceBySearchTermAsync(string searchTerm);
    }
}
