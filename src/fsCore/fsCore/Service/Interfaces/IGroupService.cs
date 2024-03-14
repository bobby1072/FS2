using Common.Models;

namespace fsCore.Service.Interfaces
{
    public interface IGroupService
    {
        Task<GroupMember> SaveGroupMember(GroupMember groupMember, UserWithGroupPermissionSet currentUser);
        Task<Group> GetGroupWithPositions(Guid groupId, UserWithGroupPermissionSet currentUser);
        Task<ICollection<Group>> GetAllListedGroups(int startIndex, int count);
        Task<ICollection<GroupMember>> GetGroupMembers(Guid groupId, UserWithGroupPermissionSet currentUser);
        Task<Group> GetGroup(Guid groupId);
        Task<Group> SaveGroup(Group group, UserWithGroupPermissionSet currentUser);
        Task<Group> DeleteGroup(Guid group, UserWithGroupPermissionSet currentUser);
        Task<GroupPosition> SavePosition(GroupPosition position, UserWithGroupPermissionSet currentUser);
        Task<GroupPosition> DeletePosition(GroupPosition position, UserWithGroupPermissionSet currentUser);
        Task<(ICollection<Group>, ICollection<GroupMember>)> GetAllGroupsAndMembershipsForUser(User currentUser);
        Task<int> GetGroupCount();
        Task<ICollection<Group>> GetAllSelfLeadGroups(User currentUser, int startIndex, int count);
    }
}