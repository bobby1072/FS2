using Common.Models;

namespace fsCore.Service.Interfaces
{
    public interface IGroupService
    {
        Task<ICollection<Group>> GetAllListedGroups();
        Task<ICollection<Group>> GetAllListedGroups(int startIndex, int count);
        Task<Group> GetGroup(Guid groupId);
        Task<GroupMember> UserChangePositionInGroup(GroupMember newMember, UserWithGroupPermissionSet currentUser);
        Task<ICollection<GroupPosition>> GetAllPositionsForGroup(UserWithGroupPermissionSet currentUser, Guid groupId);
        Task<GroupMember> GetMembership(UserWithGroupPermissionSet currentUser, string targetUserEmail, Guid groupId, bool includePosition = false, bool includeUser = false, bool includeGroup = false);
        Task<ICollection<GroupMember>> GetAllMemberships(UserWithGroupPermissionSet currentUser, string targetEmail, bool includePosition = false, bool includeUser = false, bool includeGroup = false);
        Task<ICollection<GroupMember>> GetAllMembershipsForGroup(Guid groupId, UserWithGroupPermissionSet currentUser, bool includePosition = false, bool includeUser = false);
        Task<GroupMember> UserJoinGroup(GroupMember member, UserWithGroupPermissionSet currentUser);
        Task<GroupMember> UserLeaveGroup(UserWithGroupPermissionSet currentUser, string targetUsername, Guid groupId);
        Task<Group> SaveGroup(Group group, UserWithGroupPermissionSet currentUser);
        Task<Group> DeleteGroup(Guid group, UserWithGroupPermissionSet currentUser);
        Task<GroupPosition> SavePosition(GroupPosition position, UserWithGroupPermissionSet currentUser);
        Task<Group> GetFullGroup(Guid groupId, UserWithGroupPermissionSet currentUser);
        Task<GroupPosition> DeletePosition(GroupPosition position, UserWithGroupPermissionSet currentUser);
        Task<(ICollection<Group>, ICollection<GroupMember>)> GetAllGroupsAndMembershipsForUser(User currentUser);
        Task<int> GetGroupCount();
    }
}