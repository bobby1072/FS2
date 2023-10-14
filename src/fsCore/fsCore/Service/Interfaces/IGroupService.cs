using Common.Models;

namespace fsCore.Service.Interfaces
{
    public interface IGroupService
    {
        Task<ICollection<Group>> GetAllListedGroups();
        Task<bool> IsUserInGroup(User currentUser, Guid groupId);
        Task<bool> IsUserInGroup(User currentUser, string groupName);
        Task<bool> IsUserLeader(User currentUser, Guid groupId);
        Task<bool> IsUserLeader(User currentUser, string groupName);
    }
}