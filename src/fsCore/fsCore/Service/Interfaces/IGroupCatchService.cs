using Common.Models;

namespace fsCore.Service.Interfaces
{
    public interface IGroupCatchService
    {
        Task<ICollection<GroupCatch>> GetAllSelfCatches(User currentUser);
        Task<ICollection<GroupCatch>> GetAllGroupCatches(UserWithGroupPermissionSet currentUser, Guid groupId);
        Task<ICollection<GroupCatch>> GetAllCatchesAvailableToUser(UserWithGroupPermissionSet currentUser);
        Task<GroupCatch> SaveCatch(GroupCatch groupCatch, UserWithGroupPermissionSet currentUser);
        Task<GroupCatch> DeleteCatch(GroupCatch groupCatch, UserWithGroupPermissionSet currentUser);
    }
}