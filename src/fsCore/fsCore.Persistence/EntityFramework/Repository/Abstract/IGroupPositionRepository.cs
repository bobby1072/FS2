using fsCore.Common.Models;

namespace fsCore.Persistence.EntityFramework.Repository.Abstract
{
    public interface IGroupPositionRepository
    {
        Task DeleteAll();

        Task<int> GetCount();
        Task<ICollection<GroupPosition>?> Create(ICollection<GroupPosition> groupPositionToCreate);
        Task<ICollection<GroupPosition>?> Update(ICollection<GroupPosition> groupPositionToUpdate);
        Task<ICollection<GroupPosition>?> Delete(ICollection<GroupPosition> groupPositionToDelete);
        Task<ICollection<GroupPosition>?> GetAll(params string[] relationships);
        Task<ICollection<GroupPosition>?> GetMany<T>(T field, string fieldName, ICollection<string>? relationships = null);
        Task<ICollection<GroupPosition>?> GetAllPositionsForGroup(Guid groupId);
        Task<GroupPosition?> GetOne<T>(T field, string fieldName, ICollection<string>? relationships = null);
        Task<GroupPosition?> GetOne(GroupPosition groupPosition, ICollection<string>? relationships = null);
        Task<ICollection<GroupPosition>?> GetMany(GroupPosition baseObj, ICollection<string>? relationships = null);
    }
}