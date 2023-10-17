using Common.Models;

namespace fsCore.Service.Interfaces
{
    public interface IGroupService
    {
        Task<ICollection<Group>> GetAllListedGroups();
        Task<bool> IsUserInGroup(User currentUser, Guid groupId);
        Task<bool> IsUserLeader(User currentUser, Guid groupId);
        Task<ICollection<Group>> GetAllGroupsForUser(User currentUser);
        Task<GroupMember> UserJoinPublicGroup(User currentUser, Guid groupId);
        Task<GroupMember> UserLeavePublicGroup(User currentUser, Guid groupId);
        Task<GroupMember> UserChangePositionInGroup(User currentUser, Guid groupId);

    }
}