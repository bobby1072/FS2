using Common.Models;

namespace Common.DbInterfaces.Repository
{
    public interface IGroupMemberRepository
    {
        Task<ICollection<GroupMember>?> GetFullMemberships(Guid userId, int count, int startIndex);
        Task<ICollection<GroupMember>?> Create(ICollection<GroupMember> groupMemberToCreate);
        Task<ICollection<GroupMember>?> Update(ICollection<GroupMember> groupMemberToUpdate);
        Task<ICollection<GroupMember>?> Delete(ICollection<GroupMember> groupMemberToDelete);
        Task<ICollection<GroupMember>?> GetAll(params string[] relationships);
        Task<ICollection<GroupMember>?> GetMany<T>(T field, string fieldName, ICollection<string>? relationships = null);
        Task<GroupMember?> GetOne<T>(T field, string fieldName, ICollection<string>? relationships = null);
        Task<GroupMember?> GetOne(GroupMember groupMember, ICollection<string>? relationships = null);
        Task<ICollection<GroupMember>?> GetMany(GroupMember baseObj, ICollection<string>? relationships = null);
    }
}