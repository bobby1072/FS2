using Common.Models;

namespace Common.Dbinterfaces.Repository
{
    public interface IUserRepository
    {
        Task<ICollection<User>?> Create(ICollection<User> userToCreate);
        Task<ICollection<User>?> Update(ICollection<User> userToUpdate);
        Task<ICollection<User>?> Delete(ICollection<User> userToDelete);
        Task<ICollection<User>?> GetAll(params string[] relationships);
        Task<ICollection<User>?> GetMany<T>(T field, string fieldName, ICollection<string>? relationships = null);
        Task<User?> GetOne<T>(T field, string fieldName, ICollection<string>? relationships = null);
        Task<User?> GetOne(User user, ICollection<string>? relationships = null);
        Task<ICollection<User>?> GetMany(User baseObj, ICollection<string>? relationships = null);
    }
}