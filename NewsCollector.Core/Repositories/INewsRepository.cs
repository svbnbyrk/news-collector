using NewsCollector.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewsCollector.Core.Repositories
{
    public interface INewsRepository:IRepository<News>
    {
        public Task<IEnumerable<News>> GetNewsBySourceId(int sourceId);
        public Task<IEnumerable<News>> GetNewsBySearchTermintheTitle(string searchTerm);
        public Task<News> GetNewsByUrlWithNewsKeyword(string url);
        public Task<IEnumerable<News>> GetNewsByKeywordId(int keywordId);
    }
}
