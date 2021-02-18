using NewsCollector.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewsCollector.Core.Services
{
    public interface INewsKeywordService
    {
        Task<IEnumerable<NewsKeyword>> GetAllNewsKeyword();
        Task<NewsKeyword> GetNewsKeywordById(int id);
        Task<NewsKeyword> CreateNewsKeyword(NewsKeyword newsKeyword);
        Task DeleteNewsKeyword(NewsKeyword newsKeyword);
    }
}
