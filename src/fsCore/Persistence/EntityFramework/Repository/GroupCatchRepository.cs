using System.Net;
using Common;
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
        public async Task<GroupCatch?> GetOne(Guid id)
        {
            await using var dbContext = await DbContextFactory.CreateDbContextAsync();
            var groupCatch = await dbContext.GroupCatch
                .Include(gc => gc.User)
                .Include(gc => gc.WorldFish)
                .Where(gc => gc.Id == id)
                .FirstOrDefaultAsync();
            return groupCatch?.ToRuntime();
        }
        public async Task<ICollection<PartialGroupCatch>?> GetCatchesInSquareRange(LatLng bottomLeft, LatLng topRight, Guid groupId)
        {
            await using var dbContext = await DbContextFactory.CreateDbContextAsync();
            var catches = await dbContext.GroupCatch
                .Include(gc => gc.User)
                .Include(gc => gc.WorldFish)
                .Where(c =>
                    c.Latitude > bottomLeft.Latitude &&
                    c.Latitude < topRight.Latitude &&
                    c.Longitude > bottomLeft.Longitude &&
                    c.Longitude < topRight.Longitude &&
                    c.GroupId == groupId)
                .Select(c => new { c.Species, c.Latitude, c.Longitude, c.User, c.CaughtAt, c.WorldFish, c.Weight, c.Id })
                .ToArrayAsync();
            return catches?.Select(c => new PartialGroupCatch(c.Species, c.Latitude, c.Longitude, c.WorldFish?.ToRuntime(), c.CaughtAt, c.User?.ToRuntime() ?? throw new ApiException(ErrorConstants.NoUserFound, HttpStatusCode.NotFound), c.Weight, c.Id)).ToArray();
        }
        public async Task<ICollection<PartialGroupCatch>?> GetAllPartialCatchesForGroup(Guid groupId)
        {
            await using var dbContext = await DbContextFactory.CreateDbContextAsync();
            var catches = await dbContext.GroupCatch
                .Include(gc => gc.User)
                .Include(gc => gc.WorldFish)
                .Where(c => c.GroupId == groupId)
                .Select(c => new { c.Species, c.Latitude, c.Longitude, c.User, c.CaughtAt, c.WorldFish, c.Weight, c.Id })
                .ToArrayAsync();
            return catches?.Select(c => new PartialGroupCatch(c.Species, c.Latitude, c.Longitude, c.WorldFish?.ToRuntime(), c.CaughtAt, c.User?.ToRuntime() ?? throw new ApiException(ErrorConstants.NoUserFound, HttpStatusCode.NotFound), c.Weight, c.Id)).ToArray();
        }
        public async Task<GroupCatch?> GetOne(LatLng latLng, Guid groupId)
        {
            await using var dbContext = await DbContextFactory.CreateDbContextAsync();
            var groupCatch = await dbContext.GroupCatch
                .Include(gc => gc.WorldFish)
                .Include(gc => gc.User)
                .FirstOrDefaultAsync(gc => gc.Latitude == latLng.Latitude && gc.Longitude == latLng.Longitude && gc.GroupId == groupId);
            return groupCatch?.ToRuntime();
        }
    }
}