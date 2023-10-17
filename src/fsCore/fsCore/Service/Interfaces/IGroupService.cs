using Common.Models;

namespace fsCore.Service.Interfaces
{
    public interface IGroupService
    {
        Task<ICollection<Group>> GetAllListedGroups();
        Task<bool> IsUserInGroup(User currentUser, Guid groupId);
        Task<ICollection<GroupPosition>> GetAllPositionsForGroup(Guid groupId);
        Task<bool> IsUserLeader(User currentUser, Guid groupId);
        Task<GroupMember> UserMembershipIncludingPosition(User currentUser, Guid groupId);
        Task<ICollection<Group>> GetAllGroupsForUser(User currentUser);
        Task<ICollection<GroupMember>> GetAllMembersForGroup(Guid groupId);
        Task<ICollection<GroupMember>> GetAllMembersForGroupIncludingUser(Guid groupId);
        Task<ICollection<GroupMember>> GetAllMembersForGroupIncludingUserAndPosition(Guid groupId);
        Task<GroupMember> UserJoinPublicGroup(User currentUser, Guid groupId);
        Task<GroupMember> UserLeavePublicGroup(User currentUser, Guid groupId);
        Task<GroupMember> UserChangePositionInGroup(User currentUser, User targetUser, Guid groupId, GroupPosition newPosition);

    }
}