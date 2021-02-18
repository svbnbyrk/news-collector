using NewsCollector.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewsCollector.Core.Repositories
{
    public interface INewsKeywordRepository:IRepository<NewsKeyword>
    {
        Task<NewsKeyword> GetNewsKeywordsbyKeywordId(int keywordId);

        Task<IEnumerable<News>> GetNewsByKeywordValue(string keywordValue);
    }
}
