using System.Collections.Generic;
using System.Threading.Tasks;
using NewsCollector.Core.Models;

namespace NewsCollector.Core.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAll();
        Task<User> GetUserById(int id);
        Task<User> CreateUser(User user);
        Task DeleteUser(User user);
        Task UpdateUser(User updatedUser, User user);
        Task<User> GetUserByUsernamePassword(string username, string password); 
    }
}