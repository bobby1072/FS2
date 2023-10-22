using Common.Models;

namespace Common.Dbinterfaces.Repository
{
    public interface IGroupPositionRepository
    {
        Task<ICollection<GroupPosition>?> Create(ICollection<GroupPosition> groupPositionToCreate);
        Task<ICollection<GroupPosition>?> Update(ICollection<GroupPosition> groupPositionToUpdate);
        Task<ICollection<GroupPosition>?> Delete(ICollection<GroupPosition> groupPositionToDelete);
        Task<ICollection<GroupPosition>?> GetAll(ICollection<string>? relationships = null);
        Task<GroupPosition?> GetOne(IDictionary<string, object> fieldAndName, ICollection<string>? relationships = null);
        Task<ICollection<GroupPosition>?> GetMany<T>(T field, string fieldName, ICollection<string>? relationships = null);
        Task<GroupPosition?> GetOne<T>(T field, string fieldName, ICollection<string>? relationships = null);
        Task<GroupPosition?> GetOne(GroupPosition groupPosition, ICollection<string>? relationships = null);
        Task<ICollection<GroupPosition>?> GetMany(GroupPosition baseObj, ICollection<string>? relationships = null);
    }
}