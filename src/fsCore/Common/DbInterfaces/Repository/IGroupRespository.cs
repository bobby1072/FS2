using Common.Models;

namespace Common.Dbinterfaces.Repository
{
    public interface IGroupRepository
    {
        Task<ICollection<Group>?> ManyGroupWithoutEmblem(Guid leaderId, ICollection<string>? relations = null);
        Task<ICollection<Group>?> Create(ICollection<Group> groupToCreate);
        Task<ICollection<Group>?> Update(ICollection<Group> groupToUpdate);
        Task<Group?> GetGroupWithoutEmblem(Guid groupId, ICollection<string>? relations = null);
        Task<ICollection<Group>?> Delete(ICollection<Group> groupToDelete);
        Task<Group?> GetOne<T>(T field, string fieldName, ICollection<string>? relationships = null);
        Task<ICollection<Group>?> GetMany<T>(int startIndex, int count, T field, string fieldName, string fieldNameToOrderBy, ICollection<string>? relations = null);
        Task<int> GetCount();
        Task<ICollection<Group>?> SearchListedGroups(string groupNameString);
    }
}