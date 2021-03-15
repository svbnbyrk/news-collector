using NewsCollector.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewsCollector.Core
{
    public interface IUnitOfWork : IDisposable
    {
        ISourceRepository Sources { get; }
        IKeywordRepository Keywords { get; }
        INewsKeywordRepository NewsKeywords { get; }
        INewsRepository News { get; }
        IUserRepository Users{get;}

        Task<int> CommitAsync();

    }
}
