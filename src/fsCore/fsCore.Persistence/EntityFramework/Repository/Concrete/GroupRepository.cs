using fsCore.Common.Models;
using fsCore.Common.Utils;
using fsCore.Persistence.EntityFramework;
using fsCore.Persistence.EntityFramework.Entity;
using fsCore.Persistence.EntityFramework.Repository.Abstract;
using Microsoft.EntityFrameworkCore;

namespace fsCore.Persistence.EntityFramework.Repository.Concrete
{
    internal class GroupRepository : BaseRepository<GroupEntity, Group>, IGroupRepository
    {
        public GroupRepository(IDbContextFactory<FsContext> context) : base(context) { }
        protected override GroupEntity RuntimeToEntity(Group runtimeObj) => GroupEntity.RuntimeToEntity(runtimeObj);
        public async Task<ICollection<Group>?> GetMany<T>(int startIndex, int count, T field, string fieldName, string fieldNameToOrderBy, ICollection<string>? relations = null)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var runtimeArray = (await _addRelationsToQuery(dbContext.Group, relations)
                .Where(x => EF.Property<T>(x, fieldName.ToPascalCase()).Equals(field))
                .OrderBy(x => EF.Property<object>(x, fieldNameToOrderBy.ToPascalCase()))
                .Skip(startIndex)
                .Take(count)
                .ToArrayAsync()
            ).Select(x => x.ToRuntime());
            return runtimeArray?.Count() > 0 ? runtimeArray.ToArray() : null;
        }
        public async Task<ICollection<Group>?> ManyGroupWithoutEmblem(Guid leaderId, ICollection<string>? relations = null)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var foundEnts = await _addRelationsToQuery(dbContext.Group, relations)
                .Where(x => x.LeaderId == leaderId)
                .Select(x => new { x.Name, x.Description, x.Id, x.CreatedAt, x.Public, x.Listed, x.LeaderId, x.Leader, x.Catches, x.Positions, x.CatchesPublic })
                .ToArrayAsync();
            return foundEnts?.Length > 0 ? foundEnts.Select(x => new Group(x.Name, null, x.Description, x.Id, x.CreatedAt, x.Public, x.Listed, x.CatchesPublic, x.LeaderId, x.Leader?.ToRuntime(), x.Positions?.Select(p => p.ToRuntime()).ToArray())).ToArray() : Array.Empty<Group>();
        }
        public async Task<Group?> GetGroupWithoutEmblem(Guid groupId, ICollection<string>? relations = null)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var group = await _addRelationsToQuery(dbContext.Group, relations)
                .Where(x => x.Id == groupId)
                .Select(x => new { x.Name, x.Description, x.Id, x.CreatedAt, x.Public, x.Listed, x.LeaderId, x.Leader, x.Catches, x.Positions, x.CatchesPublic })
                .FirstOrDefaultAsync();
            return group is null ? null : new Group(group.Name, null, group.Description, group.Id, group.CreatedAt, group.Public, group.Listed, group.CatchesPublic, group.LeaderId, group.Leader?.ToRuntime(), group.Positions?.Select(p => p.ToRuntime()).ToArray());
        }
        public async Task<ICollection<Group>?> SearchListedGroups(string groupNameString)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var foundGroup = await dbContext
                .Group
                .Where(x => x.Listed == true && x.Name.ToLower().Contains(groupNameString.ToLower()))
                .Take(5)
                .ToArrayAsync();
            return foundGroup?.Select(x => x.ToRuntime()).ToArray();
        }
        public async Task<ICollection<Group>?> GetGroupWithoutEmblem(ICollection<Guid> groupId, ICollection<string>? relations = null)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var foundEnts = await _addRelationsToQuery(dbContext.Group, relations)
                .Where(x => groupId.Contains(x.Id))
                .Select(x => new { x.Name, x.Description, x.Id, x.CreatedAt, x.Public, x.Listed, x.LeaderId, x.Leader, x.Catches, x.Positions, x.CatchesPublic })
                .ToArrayAsync();
            return foundEnts?.Length > 0 ? foundEnts.Select(x => new Group(x.Name, null, x.Description, x.Id, x.CreatedAt, x.Public, x.Listed, x.CatchesPublic, x.LeaderId, x.Leader?.ToRuntime(), x.Positions?.Select(p => p.ToRuntime()).ToArray())).ToArray() : Array.Empty<Group>();
        }
    }

}