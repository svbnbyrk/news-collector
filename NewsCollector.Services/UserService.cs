using System.Collections.Generic;
using System.Threading.Tasks;
using NewsCollector.Core;
using NewsCollector.Core.Models;
using NewsCollector.Core.Services;

namespace NewsCollector.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<User> CreateUser(User user)
        {
            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CommitAsync();
            return user;
        }

        public async Task DeleteUser(User user)
        {
             _unitOfWork.Users.Remove(user);
             await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<User>> GetAll()
        {
           return await _unitOfWork.Users.GetAllAsync();
        }

        public async Task<User> GetUserById(int id)
        {
            return await _unitOfWork.Users.GetByIdAsync(id);
        }

        public async Task<User> GetUserByUsernamePassword(string username, string password)
        {
            return await _unitOfWork.Users.GetUserByUserNamePassword(username, password);
        }

        public async Task UpdateUser(User updatedUser, User user)
        {
            updatedUser.FirstName = user.FirstName;
            updatedUser.LastName = user.LastName;
            updatedUser.Password = user.Password;
            updatedUser.Username = user.Username;
            
            await _unitOfWork.CommitAsync();
        }
    }
}