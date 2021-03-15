using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NewsCollector.Core.Models;
using NewsCollector.Core.Repositories;

namespace NewsCollector.Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(NewsCollectorDbContext context) : base(context)
        { }

        public async Task<User> GetUserByUserNamePassword(string username, string password)
        {
            return await NewsCollectorDbContext.Users.SingleOrDefaultAsync(x => x.Username == username && x.Password == password);
        }
        private NewsCollectorDbContext NewsCollectorDbContext
        {
            get { return Context as NewsCollectorDbContext; }
        }
    }
}