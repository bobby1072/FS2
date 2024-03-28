using Common.Dbinterfaces.Repository;
using Common.Models;
using Common.Models.MiscModels;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;

namespace Persistence.EntityFramework.Repository
{
    internal class GroupCatchRepository : BaseRepository<GroupCatchEntity, GroupCatch>, IGroupCatchRepository
    {
        public GroupCatchRepository(IDbContextFactory<FsContext> context) : base(context) { }
        protected override GroupCatchEntity _runtimeToEntity(GroupCatch runtimeObj)
        {
            return GroupCatchEntity.RuntimeToEntity(runtimeObj);
        }
        public async Task<ICollection<PartialGroupCatch>?> GetCatchesInSquareRange(LatLng bottomLeft, LatLng topRight, Guid groupId)
        {
            await using var dbContext = await DbContextFactory.CreateDbContextAsync();
            var catches = await dbContext.GroupCatch
                .Where(c =>
                    c.Latitude > bottomLeft.Latitude &&
                    c.Latitude < topRight.Latitude &&
                    c.Longitude > bottomLeft.Longitude &&
                    c.Longitude < topRight.Longitude &&
                    c.GroupId == groupId)
                .Select(c => new { c.Species, c.Latitude, c.Longitude })
                .ToArrayAsync();
            return catches?.Select(c => new PartialGroupCatch(c.Species, c.Latitude, c.Longitude)).ToArray();
        }
        public async Task<GroupCatch?> GetOneFull(LatLng latLng, Guid groupId)
        {
            await using var dbContext = await DbContextFactory.CreateDbContextAsync();
            var groupCatch = await dbContext.GroupCatch
                .Include(gc => gc.WorldFish)
                .FirstOrDefaultAsync(gc => gc.Latitude == latLng.Latitude && gc.Longitude == latLng.Longitude && gc.GroupId == groupId);
            return groupCatch?.ToRuntime();
        }
    }
}