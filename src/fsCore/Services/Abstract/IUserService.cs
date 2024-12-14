using fsCore.Common.Models;

namespace Services.Abstract
{
    public interface IUserService
    {
        Task<User> SaveUser(User user);
        Task<User> GetUser(Guid id);
        Task<User> GetUser(Guid id, UserWithGroupPermissionSet currentUser);
        Task<ICollection<User>> GetUser(ICollection<Guid> id);
        Task<string> FindUniqueUsername(User user);
        Task<User> CheckUserExistsAndCreateIfNot(User user);
        Task<ICollection<UserWithoutEmail>> SearchUsers(string searchTerm);
    }
}