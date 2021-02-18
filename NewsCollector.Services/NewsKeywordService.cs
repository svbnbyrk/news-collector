using NewsCollector.Core;
using NewsCollector.Core.Models;
using NewsCollector.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewsCollector.Services
{
    public class NewsKeywordService : INewsKeywordService
    {
        private readonly IUnitOfWork _unitOfWork;
        public NewsKeywordService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<NewsKeyword> CreateNewsKeyword(NewsKeyword newsKeyword)
        {
           await _unitOfWork.NewsKeywords.AddAsync(newsKeyword);
            await _unitOfWork.CommitAsync();
            return (newsKeyword);
        }

        public async Task DeleteNewsKeyword(NewsKeyword newsKeyword)
        {
            _unitOfWork.NewsKeywords.Remove(newsKeyword);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<NewsKeyword>> GetAllNewsKeyword()
        {
            return await _unitOfWork.NewsKeywords.GetAllAsync();  
        }

        public async Task<NewsKeyword> GetNewsKeywordById(int id)
        {
            return await _unitOfWork.NewsKeywords.GetByIdAsync(id);
        }
    }
}
