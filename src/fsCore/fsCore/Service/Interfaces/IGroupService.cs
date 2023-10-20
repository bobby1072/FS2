using Common.Models;

namespace fsCore.Service.Interfaces
{
    public interface IGroupService
    {
        Task<ICollection<Group>> GetAllListedGroups();
        Task<bool> IsUserInGroup(User currentUser, Guid groupId);
        Task<bool> IsUserLeader(User currentUser, Guid groupId);
        Task<ICollection<GroupPosition>> GetAllPositionsForGroup(Guid groupId);
        Task<GroupMember> UserMembershipIncludingPosition(User currentUser, Guid groupId);
        Task<ICollection<GroupMember>> GetAllMembersForUserIncludingGroupAndUserAndPosition(User currentUser);
        Task<ICollection<GroupMember>> GetAllMembersForGroup(Guid groupId);
        Task<ICollection<GroupMember>> GetAllMembersForGroupIncludingUser(Guid groupId);
        Task<ICollection<GroupMember>> GetAllMembersForGroupIncludingUserAndPosition(Guid groupId);
        Task<GroupMember> UserJoinPublicGroup(GroupMember member);
        Task<GroupMember> UserLeavePublicGroup(User currentUser, Guid groupId);
        Task<GroupMember> UserChangePositionInGroup(GroupMember newMember);
        Task<ICollection<GroupMember>>? TryGetAllMembersForUserIncludingGroupAndUserAndPosition(User currentUser);

    }
}