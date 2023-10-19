using Common.Models;

namespace Common.Dbinterfaces.Repository
{
    public interface IGroupRepository
    {
        Task<ICollection<Group>?> GetAll();
        Task<ICollection<Group>?> Create(ICollection<Group> groupModelToCreate);
        Task<ICollection<Group>?> Update(ICollection<Group> groupModelToUpdate);
        Task<ICollection<Group>?> Delete(ICollection<Group> groupModelToDelete);
        Task<Group?> GetOne<T>(T field, string fieldName);
        Task<Group?> GetOne(Group groupModel);
        Task<ICollection<Group>?> GetMany<T>(T field, string fieldName);
        Task<ICollection<Group>?> GetMany(Group groupModel);

    }
}