using Common.Models;

namespace Common.Dbinterfaces.Repository
{
    public interface IGroupRepository
    {
        Task<ICollection<Group>?> Create(ICollection<Group> groupToCreate);
        Task<ICollection<Group>?> Update(ICollection<Group> groupToUpdate);
        Task<ICollection<Group>?> Delete(ICollection<Group> groupToDelete);
        Task<Group?> GetOne(IDictionary<string, object> fieldAndName, ICollection<string>? relationships = null);
        Task<ICollection<Group>?> GetAll(ICollection<string>? relationships = null);
        Task<ICollection<Group>?> GetMany<T>(T field, string fieldName, ICollection<string>? relationships = null);
        Task<Group?> GetOne<T>(T field, string fieldName, ICollection<string>? relationships = null);
        Task<Group?> GetOne(Group group, ICollection<string>? relationships = null);
        Task<ICollection<Group>?> GetMany(Group baseObj, ICollection<string>? relationships = null);

    }
}