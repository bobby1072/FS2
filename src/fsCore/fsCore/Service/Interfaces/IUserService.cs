using Common.Models;

namespace fsCore.Service.Interfaces
{
    public interface IUserService
    {
        Task<User> SaveUser(User user);
        Task<User> GetUser(string email);
        Task<User> GetUser(Guid id);
        Task<string> FindUniqueUsername(User user);
        Task<User> CheckUserExistsAndCreateIfNot(User user);
        Task<ICollection<UserWithoutEmail>> SearchUsers(string searchTerm);
    }
}