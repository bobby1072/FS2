using Common.Models;

namespace Persistence.EntityFramework.Abstract.Repository
{
    public interface IUserRepository
    {
        Task DeleteAll();
        Task<ICollection<User>> GetUsers(ICollection<Guid> ids);
        Task<int> GetCount();
        Task<ICollection<UserWithoutEmail>> FindManyLikeWithSensitiveRemoved(string searchTerm);
        Task<bool> IsUserNameUnique(User runtimeObj);
        Task<ICollection<User>?> Create(ICollection<User> userToCreate);
        Task<ICollection<User>?> Update(ICollection<User> userToUpdate);
        Task<ICollection<User>?> Delete(ICollection<User> userToDelete);
        Task<ICollection<User>?> GetAll(params string[] relationships);
        Task<User?> GetOne<T>(T field, string fieldName, ICollection<string>? relationships = null);
    }
}