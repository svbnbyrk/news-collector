using NewsCollector.Core;
using NewsCollector.Core.Domain;
using NewsCollector.Core.Models;
using NewsCollector.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewsCollector.Services
{
    public class NewsService : INewsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NewsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<News> CreateNews(News news)
        {
            await _unitOfWork.News.AddAsync(news);
            await _unitOfWork.CommitAsync();
            return news;
        }

        public async Task DeleteNews(News news)
        {
            _unitOfWork.News.Remove(news);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<News>> GetAllNews(PaginationFilter pagination = null)
        {
            if(pagination == null)
            {
                return await _unitOfWork.News.GetAllAsync();
            }
            return await _unitOfWork.News.GetAllAsync(pagination);
        }

        public async Task<News> GetNewsById(int id)
        {
            return await _unitOfWork.News.GetByIdAsync(id);
        }

        public async Task<News> GetNewsByUrlWithNewsKeyword(string url)
        {
            return await _unitOfWork.News.GetNewsByUrlWithNewsKeyword(url);
        }

        public async Task<IEnumerable<News>> GetNewsBySourceId(int id)
        {
            return await _unitOfWork.News.GetNewsBySourceId(id);
        }

        public async Task UpdateNews(News updatedNews, News news)
        {
            updatedNews.NewsDate = news.NewsDate;
            updatedNews.NewsTitle = news.NewsTitle;
            updatedNews.NewsUrl = news.NewsUrl;
            updatedNews.SourceId = news.SourceId;

            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<News>> GetNewsByKeywordId(int keywordId)
        {
            return await _unitOfWork.News.GetNewsByKeywordId(keywordId);
        }
    }
}
