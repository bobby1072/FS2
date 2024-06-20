using Common.Models;

namespace fsCore.Service.Interfaces
{
    public interface IGroupService
    {
        Task<ICollection<Group>> GetAllGroupsForUser(User currentUser, int startIndex, int count);
        Task<ICollection<Group>> SearchAllListedGroups(string groupNameString);
        Task<GroupPosition> DeletePosition(int positionId, UserWithGroupPermissionSet currentUser);
        Task<GroupMember> DeleteGroupMember(int groupMemberId, UserWithGroupPermissionSet currentUser);
        Task<GroupMember> SaveGroupMember(GroupMember groupMember, UserWithGroupPermissionSet currentUser);
        Task<Group> GetGroupWithPositions(Guid groupId, UserWithGroupPermissionSet currentUser);
        Task<ICollection<Group>> GetAllListedGroups(int startIndex, int count);
        Task<ICollection<GroupMember>> GetGroupMembers(Guid groupId, UserWithGroupPermissionSet currentUser);
        Task<Group> GetGroupWithoutEmblemForInternalUse(Guid groupId);
        Task<ICollection<Group>> GetGroupsWithoutEmblemForInternalUse(ICollection<Guid> groupId);
        Task<Group> SaveGroup(Group group, UserWithGroupPermissionSet currentUser);
        Task<Group> DeleteGroup(Guid group, UserWithGroupPermissionSet currentUser);
        Task<GroupPosition> SavePosition(GroupPosition position, UserWithGroupPermissionSet currentUser);
        Task<(ICollection<Group>, ICollection<GroupMember>)> GetAllGroupsAndMembershipsForUser(User currentUser);
        Task<int> GetGroupCount();
        Task<ICollection<Group>> GetAllSelfLeadGroups(User currentUser, int startIndex, int count);
    }
}