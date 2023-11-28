using Common.Models;

namespace Common.Dbinterfaces.Repository
{
    public interface IGroupCatchRepository
    {
        Task<ICollection<GroupCatch>?> Create(ICollection<GroupCatch> GroupCatchToCreate);
        Task<ICollection<GroupCatch>?> Update(ICollection<GroupCatch> GroupCatchToUpdate);
        Task<ICollection<GroupCatch>?> Delete(ICollection<GroupCatch> GroupCatchToDelete);
        Task<ICollection<GroupCatch>?> GetAll(params string[] relationships);
        Task<ICollection<GroupCatch>?> GetMany<T>(T field, string fieldName, ICollection<string>? relationships = null);
        Task<GroupCatch?> GetOne<T>(T field, string fieldName, ICollection<string>? relationships = null);
        Task<GroupCatch?> GetOne(GroupCatch GroupCatch, ICollection<string>? relationships = null);
        Task<ICollection<GroupCatch>?> GetMany(GroupCatch baseObj, ICollection<string>? relationships = null);
        Task<GroupCatch?> GetOne(IDictionary<string, object> fieldAndName, ICollection<string>? relationships = null);
    }
}