using NewsCollector.Core;
using NewsCollector.Core.Models;
using NewsCollector.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewsCollector.Services
{
    public class KeywordService : IKeywordService
    {
        private readonly IUnitOfWork _unitOfWork;
        public KeywordService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Keyword> CreateKeyword(Keyword keyword)
        {
            await _unitOfWork.Keywords.AddAsync(keyword);
            await _unitOfWork.CommitAsync();
            return keyword;
        }

        public async Task DeleteKeyword(Keyword keyword)
        {
            _unitOfWork.Keywords.Remove(keyword);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<Keyword>> GetAllKeywords()
        {
            return await _unitOfWork.Keywords.GetAllAsync();
        }

        public async Task<Keyword> GetKeywordById(int id)
        {
            return await _unitOfWork.Keywords.GetByIdAsync(id);
        }

        public async Task UpdateKeyword(Keyword updatedKeyword, Keyword keyword)
        {
            updatedKeyword.KeywordValue = keyword.KeywordValue;
            await _unitOfWork.CommitAsync();
        }
    }
}
