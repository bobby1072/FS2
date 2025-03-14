using fsCore.Common.Misc;
using fsCore.Common.Models;
using fsCore.Persistence.EntityFramework;
using fsCore.Persistence.EntityFramework.Entity;
using fsCore.Persistence.EntityFramework.Repository.Abstract;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace fsCore.Persistence.EntityFramework.Repository.Concrete
{
    internal class GroupCatchRepository : BaseRepository<GroupCatchEntity, GroupCatch>, IGroupCatchRepository
    {
        public GroupCatchRepository(IDbContextFactory<FsContext> context) : base(context) { }
        protected override GroupCatchEntity RuntimeToEntity(GroupCatch runtimeObj)
        {
            return GroupCatchEntity.RuntimeToEntity(runtimeObj);
        }
        public async Task<PartialGroupCatch?> GetOnePartial(Guid id)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var groupCatch = await dbContext.GroupCatch
                .Include(gc => gc.User)
                .Include(gc => gc.WorldFish)
                .Select(c => new { c.Species, c.Latitude, c.Longitude, c.User, c.CaughtAt, c.WorldFish, c.Weight, c.Id, c.Length, c.GroupId })
                .FirstOrDefaultAsync(gc => gc.Id == id);

            return groupCatch is null ? null : new PartialGroupCatch(groupCatch.Species, groupCatch.Latitude, groupCatch.Longitude, groupCatch.WorldFish?.ToRuntime(), groupCatch.CaughtAt, groupCatch.User?.ToRuntime() ?? throw new ApiException(ErrorConstants.NoUserFound, HttpStatusCode.NotFound), groupCatch.Weight, groupCatch.Id, groupCatch.Length, groupCatch.GroupId);
        }
        public async Task<GroupCatch?> GetOne(Guid id)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var groupCatch = await dbContext.GroupCatch
                .Include(gc => gc.User)
                .Include(gc => gc.WorldFish)
                .Include(gc => gc.Group)
                .Where(gc => gc.Id == id)
                .Select(gc => new
                {
                    gc.Id,
                    gc.GroupId,
                    GroupName = gc.Group.Name,
                    GroupDescription = gc.Group.Description,
                    GroupCatchesPublic = gc.Group.CatchesPublic,
                    GroupListed = gc.Group.Listed,
                    GroupPublic = gc.Group.Public,
                    GroupCreatedAt = gc.Group.CreatedAt,
                    GroupLeaderId = gc.Group.LeaderId,
                    gc.UserId,
                    gc.User,
                    gc.Species,
                    gc.Weight,
                    gc.Length,
                    gc.Description,
                    gc.CreatedAt,
                    gc.CaughtAt,
                    gc.CatchPhoto,
                    gc.Latitude,
                    gc.Longitude,
                    gc.WorldFishTaxocode,
                    gc.WorldFish
                })
                .FirstOrDefaultAsync() ?? throw new ApiException(ErrorConstants.NoFishFound, HttpStatusCode.NotFound);
            var newGroup = new Group(groupCatch.GroupName, null, groupCatch.GroupDescription, groupCatch.GroupId, groupCatch.GroupCreatedAt, groupCatch.GroupPublic, groupCatch.GroupListed, groupCatch.GroupCatchesPublic, groupCatch.GroupLeaderId);
            return new GroupCatch(groupCatch.UserId, groupCatch.GroupId, groupCatch.Species, groupCatch.Weight, groupCatch.CaughtAt, groupCatch.Length, groupCatch.Latitude, groupCatch.Longitude, groupCatch.Description, groupCatch.Id, groupCatch.CreatedAt, groupCatch.CatchPhoto, newGroup, groupCatch.User?.ToRuntime(), groupCatch.WorldFishTaxocode, groupCatch.WorldFish?.ToRuntime());
        }
        public async Task<ICollection<PartialGroupCatch>?> GetAllPartialCatchesForUser(Guid userId)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var catches = await dbContext.GroupCatch
                .Include(gc => gc.User)
                .Include(gc => gc.WorldFish)
                .Where(c => c.UserId == userId)
                .Select(c => new { c.Species, c.Latitude, c.Longitude, c.User, c.CaughtAt, c.WorldFish, c.Weight, c.Id, c.Length, c.GroupId })
                .ToArrayAsync();
            return catches?.Select(c => new PartialGroupCatch(c.Species, c.Latitude, c.Longitude, c.WorldFish?.ToRuntime(), c.CaughtAt, c.User?.ToRuntime() ?? throw new ApiException(ErrorConstants.NoUserFound, HttpStatusCode.NotFound), c.Weight, c.Id, c.Length, c.GroupId)).ToArray();
        }
        public async Task<ICollection<PartialGroupCatch>?> GetCatchesInSquareRange(LatLng bottomLeft, LatLng topRight, Guid groupId)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var catches = await dbContext.GroupCatch
                .Include(gc => gc.User)
                .Include(gc => gc.WorldFish)
                .Where(c =>
                    c.Latitude > bottomLeft.Latitude &&
                    c.Latitude < topRight.Latitude &&
                    c.Longitude > bottomLeft.Longitude &&
                    c.Longitude < topRight.Longitude &&
                    c.GroupId == groupId)
                .Select(c => new { c.Species, c.Latitude, c.Longitude, c.User, c.CaughtAt, c.WorldFish, c.Weight, c.Id, c.Length, c.GroupId })
                .ToArrayAsync();
            return catches?.Select(c => new PartialGroupCatch(c.Species, c.Latitude, c.Longitude, c.WorldFish?.ToRuntime(), c.CaughtAt, c.User?.ToRuntime() ?? throw new ApiException(ErrorConstants.NoUserFound, HttpStatusCode.NotFound), c.Weight, c.Id, c.Length, c.GroupId)).ToArray();
        }
        public async Task<ICollection<PartialGroupCatch>?> GetAllPartialCatchesForGroup(Guid groupId)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var catches = await dbContext.GroupCatch
                .Include(gc => gc.User)
                .Include(gc => gc.WorldFish)
                .Where(c => c.GroupId == groupId)
                .Select(c => new { c.Species, c.Latitude, c.Longitude, c.User, c.CaughtAt, c.WorldFish, c.Weight, c.Id, c.Length, c.GroupId })
                .ToArrayAsync();
            return catches?.Select(c => new PartialGroupCatch(c.Species, c.Latitude, c.Longitude, c.WorldFish?.ToRuntime(), c.CaughtAt, c.User?.ToRuntime() ?? throw new ApiException(ErrorConstants.NoUserFound, HttpStatusCode.NotFound), c.Weight, c.Id, c.Length, c.GroupId)).ToArray();
        }
        public async Task<GroupCatch?> GetOne(LatLng latLng, Guid groupId)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var groupCatch = await dbContext.GroupCatch
                .Include(gc => gc.WorldFish)
                .Include(gc => gc.User)
                .FirstOrDefaultAsync(gc => gc.Latitude == latLng.Latitude && gc.Longitude == latLng.Longitude && gc.GroupId == groupId);
            return groupCatch?.ToRuntime();
        }
    }
}