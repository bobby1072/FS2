using Common.Models;

namespace Common.Dbinterfaces.Repository
{
    public interface IGroupMemberRepository
    {
        Task<ICollection<GroupMember>?> GetAll();
        Task<ICollection<GroupMember>?> Create(ICollection<GroupMember> groupMemberToCreate);
        Task<ICollection<GroupMember>?> Update(ICollection<GroupMember> groupMemberToUpdate);
        Task<ICollection<GroupMember>?> Delete(ICollection<GroupMember> groupMemberToDelete);
        Task<GroupMember?> GetOne<T>(T field, string fieldName);
        Task<ICollection<GroupMember>?> GetMany<T>(T field, string fieldName);
        Task<GroupMember?> GetOne(GroupMember groupMember);
        Task<GroupMember?> GetGroupMemberIncludingUser(string userEmail, Guid groupId);
        Task<GroupMember?> GetGroupMemberIncludingUserAndGroup(string userEmail, Guid groupId);
        Task<GroupMember?> GetGroupMemberIncludingPosition(string userEmail, Guid groupId);
        Task<ICollection<GroupMember>?> GetManyGroupMembersIncludingUserAndPosition(Guid groupId);
        Task<ICollection<GroupMember>?> GetManyGroupMemberForUserIncludingUserAndPositionAndGroup(string userEmail);
        Task<ICollection<GroupMember>?> GetManyGroupMemberWithGroup(Guid groupId);
    }
}