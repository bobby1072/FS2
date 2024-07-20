using Common.Models;
using Common.Models.MiscModels;

namespace Persistence.EntityFramework.Repository.Abstract
{
    public interface IGroupCatchRepository
    {

        Task<int> GetCount();
        Task DeleteAll();
        Task<ICollection<GroupCatch>?> Create(ICollection<GroupCatch> GroupCatchToCreate);
        Task<ICollection<GroupCatch>?> Update(ICollection<GroupCatch> GroupCatchToUpdate);
        Task<ICollection<GroupCatch>?> Delete(ICollection<GroupCatch> GroupCatchToDelete);
        Task<GroupCatch?> GetOne(LatLng latLng, Guid groupId);
        Task<GroupCatch?> GetOne(Guid id);
        Task<PartialGroupCatch?> GetOnePartial(Guid id);
        Task<ICollection<PartialGroupCatch>?> GetAllPartialCatchesForUser(Guid userId);
        Task<ICollection<PartialGroupCatch>?> GetCatchesInSquareRange(LatLng bottomLeft, LatLng topRight, Guid groupId);
        Task<ICollection<PartialGroupCatch>?> GetAllPartialCatchesForGroup(Guid groupId);
    }
}