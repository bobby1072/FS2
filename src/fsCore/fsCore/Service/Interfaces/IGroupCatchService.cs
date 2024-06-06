using Common.Models;
using Common.Models.MiscModels;

namespace fsCore.Service.Interfaces
{
    public interface IGroupCatchService
    {
        Task<GroupCatch> DeleteGroupCatch(Guid id, UserWithGroupPermissionSet userWithGroupPermissionSet);
        Task<GroupCatch> SaveGroupCatch(GroupCatch groupCatch, UserWithGroupPermissionSet userWithGroupPermissionSet);
        Task<ICollection<PartialGroupCatch>> GetAllPartialCatchesForGroup(Guid groupId, UserWithGroupPermissionSet userWithGroupPermissionSet);
        Task<GroupCatch> GetFullCatchById(Guid catchId, UserWithGroupPermissionSet currentUser);
        Task<ICollection<PartialGroupCatch>> GetAllPartialCatchesForUser(Guid userId, UserWithGroupPermissionSet userWithGroupPermissionSet);
        Task<GroupCatchComment> CommentOnCatch(GroupCatchComment groupCatchComment, UserWithGroupPermissionSet userWithGroupPermissionSet);
        Task<GroupCatchComment> DeleteComment(int id, UserWithGroupPermissionSet userWithGroupPermissionSet);
    }
}