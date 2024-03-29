using Common.Models;
using Common.Models.MiscModels;

namespace fsCore.Service.Interfaces
{
    public interface IGroupCatchService
    {
        Task<GroupCatch> DeleteGroupCatch(Guid id, UserWithGroupPermissionSet userWithGroupPermissionSet);
        Task<GroupCatch> SaveGroupCatch(GroupCatch groupCatch, UserWithGroupPermissionSet userWithGroupPermissionSet);
        Task<GroupCatch> GetFullGroupCatchByLatAndLngWithAssociatedWorldFish(LatLng latLng, Guid groupId, UserWithGroupPermissionSet userWithGroupPermissionSet);
        Task<ICollection<PartialGroupCatch>> GetCatchesInSquareRange(LatLng bottomLeftLatLong, LatLng topRightLatLong, Guid groupId, UserWithGroupPermissionSet userWithGroupPermissionSet);
    }
}