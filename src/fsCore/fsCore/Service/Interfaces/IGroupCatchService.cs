using Common.Models;

namespace fsCore.Service.Interfaces
{
    public interface IGroupCatchService
    {
        Task<GroupCatch> SaveGroupCatch(GroupCatch groupCatch, UserWithGroupPermissionSet userWithGroupPermissionSet);
        Task<GroupCatch> GetFullGroupCatchByLatAndLong(double latitude, double longitude, Guid groupId, UserWithGroupPermissionSet userWithGroupPermissionSet);
        Task<ICollection<GroupCatch>> GetCatchesInSquareRange(double topLeftLatLong);
    }
}