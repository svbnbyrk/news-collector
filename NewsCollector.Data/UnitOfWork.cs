using NewsCollector.Core;
using NewsCollector.Core.Repositories;
using NewsCollector.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewsCollector.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly NewsCollectorDbContext _context;
        private SourceRepository _sourceRepository;
        private NewsRepository _newsRepository;
        private NewsKeywordRepository _newsKeywordRepository;
        private KeywordRepository _keywordRepository;

        public UnitOfWork(NewsCollectorDbContext context)
        {
            _context = context;
        }

        public ISourceRepository Sources => _sourceRepository ?? new SourceRepository(_context);

        public IKeywordRepository Keywords => _keywordRepository ?? new KeywordRepository(_context);

        public INewsKeywordRepository NewsKeywords => _newsKeywordRepository ?? new NewsKeywordRepository(_context);

        public INewsRepository News => _newsRepository ?? new NewsRepository(_context);

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
