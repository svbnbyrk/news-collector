using NewsCollector.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewsCollector.Core.Services
{
    public interface ISourceService
    {
        Task<IEnumerable<Source>> GetAllSources();
        Task<Source> GetSourceById(int id);
        Task<Source> GetSourceBySearchTermAsync(string searchTerm);        
        Task<Source> CreateSource(Source newSource);
        Task UpdateSource(Source updatedSource, Source source);
        Task DeleteSource(Source source);
    }
}
