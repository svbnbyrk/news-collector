using NewsCollector.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewsCollector.Core.Repositories
{
    public interface IKeywordRepository:IRepository<Keyword>
    {
        Task<Keyword> GetWithNewsKeywordByIdAsync(int id);
    }
}
