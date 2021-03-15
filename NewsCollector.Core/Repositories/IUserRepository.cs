using System.Threading.Tasks;
using NewsCollector.Core.Models;

namespace NewsCollector.Core.Repositories
{
    public interface IUserRepository:IRepository<User>
    {
        Task<User> GetUserByUserNamePassword(string username, string password); 
    }
}