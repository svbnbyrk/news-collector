using NewsCollector.Core;
using NewsCollector.Core.Models;
using NewsCollector.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewsCollector.Services
{
    public class SourceService : ISourceService
    {
        //dependency injection
        private readonly IUnitOfWork _unitOfWork;

        public SourceService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<Source> CreateSource(Source newSource)
        {
            await _unitOfWork.Sources
                .AddAsync(newSource);
            await _unitOfWork.CommitAsync();
            return newSource;
        }

        public async Task DeleteSource(Source source)
        {
            _unitOfWork.Sources.Remove(source);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<Source>> GetAllSources()
        {
            return await _unitOfWork.Sources.GetAllAsync();
        }

        public async Task<Source> GetSourceById(int id)
        {
            return await _unitOfWork.Sources.GetByIdAsync(id);
        }

        public async Task<Source> GetSourceBySearchTermAsync(string searchTerm)
        {
            return await _unitOfWork.Sources.GetSourceBySearchTermAsync(searchTerm);
        }

        public async Task UpdateSource(Source updatedSource, Source source)
        {
            updatedSource.SourceName = source.SourceName;
            updatedSource.WebAdress = source.WebAdress;
            await _unitOfWork.CommitAsync();
        }
    }
}
