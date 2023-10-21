using Common.Models;

namespace fsCore.Service.Interfaces
{
    public interface IGroupService
    {
        Task<ICollection<Group>> GetAllListedGroups();
        Task<GroupMember> UserChangePositionInGroup(GroupMember newMember, UserWithGroupPermissionSet currentUser);
        Task<ICollection<GroupPosition>> GetAllPositionsForGroup(UserWithGroupPermissionSet currentUser, Guid groupId);
        Task<GroupMember> GetMembership(UserWithGroupPermissionSet currentUser, Guid groupId, bool includePosition = false, bool includeUser = false, bool includeGroup = false);
        Task<ICollection<GroupMember>> GetAllMemberships(User currentUser, bool includePosition = false, bool includeUser = false, bool includeGroup = false);
        Task<ICollection<GroupMember>> GetAllMemberships(UserWithGroupPermissionSet currentUser, bool includePosition = false, bool includeUser = false, bool includeGroup = false);
        Task<GroupMember> UserJoinGroup(GroupMember member, UserWithGroupPermissionSet currentUser);
        Task<GroupMember> UserLeaveGroup(UserWithGroupPermissionSet currentUser, Guid groupId);
        Task<ICollection<GroupMember>>? TryGetAllMemberships(User currentUser, bool includePosition = false, bool includeUser = false, bool includeGroup = false);
        Task<ICollection<GroupMember>> GetAllMembershipsForGroup(Guid groupId, UserWithGroupPermissionSet currentUser, bool includePosition = false, bool includeUser = false, bool includeGroup = false);
        Task<Group> CreateGroup(Group group, UserWithGroupPermissionSet currentUser);
        Task<Group> UpdateGroup(Group group, UserWithGroupPermissionSet currentUser);
        Task<Group> DeleteGroup(Group group, UserWithGroupPermissionSet currentUser);

    }
}