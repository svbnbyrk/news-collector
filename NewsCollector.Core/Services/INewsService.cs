using NewsCollector.Core.Domain;
using NewsCollector.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewsCollector.Core.Services
{
    public interface INewsService
    {
        Task<IEnumerable<News>> GetAllNews(PaginationFilter pagination);
        Task<News> GetNewsById(int id);
        Task<News> GetNewsByUrlWithNewsKeyword(string url);
        Task<News> CreateNews(News news);
        Task UpdateNews(News updatedNews, News news);
        Task DeleteNews(News news);
        Task<IEnumerable<News>> GetNewsBySourceId(int id);
        Task<IEnumerable<News>> GetNewsByKeywordId(int keywordId);
        Task<IEnumerable<News>> GetNews(PaginationFilter pagination, string searchParam, DateTime startDate, DateTime endDate, string excludedParam);
    }
}
