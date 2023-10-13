using Common.Models;

namespace Common.Dbinterfaces.Repository
{
    public interface IGroupRepository
    {
        Task<ICollection<Group>?> GetAll();
        Task<ICollection<Group>?> Create(ICollection<Group> GroupModelToCreate);
        Task<ICollection<Group>?> Update(ICollection<Group> fishToUpdate);
        Task<ICollection<Group>?> Delete(ICollection<Group> fishToDelete);
        Task<Group?> GetOne<T>(T field, string fieldName);
        Task<Group?> GetOne(Group GroupModel);
    }
}