using Common.Dbinterfaces.Repository;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;

namespace Persistence.EntityFramework.Repository
{
    internal class GroupPositionRepository : BaseRepository<GroupPositionEntity, GroupPosition>, IGroupPositionRepository
    {
        public GroupPositionRepository(IDbContextFactory<FsContext> context) : base(context) { }
        protected override GroupPositionEntity _runtimeToEntity(GroupPosition runtimeObj)
        {
            return GroupPositionEntity.RuntimeToEntity(runtimeObj);
        }
        public async Task<ICollection<GroupPosition>?> GetAllPositionsForGroup(Guid groupId)
        {
            await using var dbContext = await DbContextFactory.CreateDbContextAsync();
            var groupPositions = await dbContext.Position
                .Include(gp => gp.Group)
                .Where(gp => gp.GroupId == groupId)
                .ToListAsync();
            return groupPositions.Select(gp => gp.ToRuntime()).ToList();
        }
    }
}