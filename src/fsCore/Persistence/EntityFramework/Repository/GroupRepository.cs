using Common.Dbinterfaces.Repository;
using Common.Models;
using Common.Utils;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;

namespace Persistence.EntityFramework.Repository
{
    internal class GroupRepository : BaseRepository<GroupEntity, Group>, IGroupRepository
    {
        public GroupRepository(IDbContextFactory<FsContext> context) : base(context) { }
        protected override GroupEntity _runtimeToEntity(Group runtimeObj) => GroupEntity.RuntimeToEntity(runtimeObj);
        public async Task<int> GetCount()
        {
            using var dbConext = await DbContextFactory.CreateDbContextAsync();
            return await dbConext.Group.CountAsync();
        }
        public async Task<ICollection<Group>?> GetMany<T>(int startIndex, int count, T field, string fieldName, string fieldNameToOrderBy, ICollection<string>? relations = null)
        {
            using var dbContext = await DbContextFactory.CreateDbContextAsync();
            var runtimeArray = (await _addRelationsToQuery(dbContext.Set<GroupEntity>(), relations)
                .Where(x => EF.Property<T>(x, fieldName.ToPascalCase()).Equals(field))
                .OrderBy(x => EF.Property<object>(x, fieldNameToOrderBy.ToPascalCase()))
                .Skip(startIndex)
                .Take(count)
                .ToArrayAsync()
            ).Select(x => x.ToRuntime());
            return runtimeArray?.Count() > 0 ? runtimeArray.ToList() : null;
        }
        public async Task<Group?> GetFullGroupWithAllRelations(Guid groupId)
        {
            using var dbContext = await DbContextFactory.CreateDbContextAsync();
            var foundGroup = await dbContext.Group
                .Include(x => x.Members)
                .ThenInclude(gm => gm.User)
                .Include(x => x.Catches)
                .Include(x => x.Positions)
                .Include(x => x.Leader)
                .Where(x => x.Id == groupId)
                .FirstOrDefaultAsync();
            return foundGroup?.ToRuntime();
        }
    }

}