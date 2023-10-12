using Common.Models;

namespace fsCore.Service
{
    public interface IUserService
    {
        Task<User> GetUser(User user);
        Task<User> GetUser(string email);
        Task<User> CreateUser(User user);
        Task<User> UpdateUser(User user);
        Task<User> DeleteUser(User user);
        Task<bool> Exists(string email);
        Task<bool> Exists(User user);
        Task<bool> ExistsAndVerified(string email);
        Task<bool> ExistsAndVerified(User user);
        Task<User> CheckUserExistsAndCreateIfNot(User user);
    }
}