using NewsCollector.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewsCollector.Core.Services
{
    public interface IKeywordService
    {
        Task<IEnumerable<Keyword>> GetAllKeywords();
        Task<Keyword> GetKeywordById(int id);
        Task<Keyword> CreateKeyword(Keyword keyword);
        Task UpdateKeyword(Keyword updatedKeyword, Keyword keyword);
        Task DeleteKeyword(Keyword keyword);
    }
}
