using Common.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;
using Persistence.EntityFramework.Repository.Abstract;

namespace Persistence.EntityFramework.Repository.Concrete
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
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var groupPositions = await dbContext.Position
                .Include(gp => gp.Group)
                .Where(gp => gp.GroupId == groupId)
                .ToArrayAsync();
            return groupPositions.Select(gp => gp.ToRuntime()).ToArray();
        }
    }
}