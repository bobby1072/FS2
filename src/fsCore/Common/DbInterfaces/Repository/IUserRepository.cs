using Common.Models;

namespace Common.Dbinterfaces.Repository
{
    public interface IUserRepository
    {
        Task<ICollection<User>?> GetAll();
        Task<ICollection<User>?> Create(ICollection<User> userToCreate);
        Task<ICollection<User>?> Update(ICollection<User> fishToUpdate);
        Task<ICollection<User>?> Delete(ICollection<User> fishToDelete);
        Task<User?> GetOne<T>(T field, string fieldName);
        Task<User?> GetOne(User user);

    }
}