using Common.DbInterfaces.Repository;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;

namespace Persistence.EntityFramework.Repository
{
    internal class GroupPositionRepository : BaseRepository<GroupPositionEntity, GroupPosition>, IGroupPositionRepository
    {
        public GroupPositionRepository(IDbContextFactory<FsContext> context) : base(context) { }
        protected override GroupPositionEntity RuntimeToEntity(GroupPosition runtimeObj)
        {
            return GroupPositionEntity.RuntimeToEntity(runtimeObj);
        }
        public async Task<ICollection<GroupPosition>?> GetAllPositionsForGroup(Guid groupId)
        {
            await using var dbContext = await DbContextFactory.CreateDbContextAsync();
            var groupPositions = await dbContext.Position
                .Include(gp => gp.Group)
                .Where(gp => gp.GroupId == groupId)
                .ToArrayAsync();
            return groupPositions.Select(gp => gp.ToRuntime()).ToArray();
        }
    }
}