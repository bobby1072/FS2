using Common.Models;

namespace fsCore.Service
{
    public interface IUserService
    {
        Task<User> GetUser(User user);
        Task<User> GetUser(string email);
        Task<User> MakeSureUserExists(User user);
    }
}