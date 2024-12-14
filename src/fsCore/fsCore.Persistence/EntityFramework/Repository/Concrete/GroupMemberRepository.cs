using fsCore.Common.Models;
using fsCore.Persistence.EntityFramework;
using fsCore.Persistence.EntityFramework.Entity;
using fsCore.Persistence.EntityFramework.Repository.Abstract;
using Microsoft.EntityFrameworkCore;

namespace fsCore.Persistence.EntityFramework.Repository.Concrete
{
    internal class GroupMemberRepository : BaseRepository<GroupMemberEntity, GroupMember>, IGroupMemberRepository
    {
        public GroupMemberRepository(IDbContextFactory<FsContext> context) : base(context) { }
        protected override GroupMemberEntity RuntimeToEntity(GroupMember runtimeObj)
        {
            return GroupMemberEntity.RuntimeToEntity(runtimeObj);
        }
        public async Task<ICollection<GroupMember>?> GetOne(ICollection<Guid> userIds, Guid groupId, bool includeUser = false)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var set = dbContext.GroupMember;
            IQueryable<GroupMemberEntity> query = set;
            if (includeUser)
            {
                query = set.Include(x => x.User);
            }
            var foundEnt = await query
                .Where(x => userIds.Contains(x.UserId) && x.GroupId == groupId)
                .ToArrayAsync();
            return foundEnt.Length > 0 ? foundEnt?.Select(x => x.ToRuntime()).ToArray() : null;
        }
        public async Task<GroupMember?> GetOne(Guid userId, Guid groupId)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var foundEnt = await dbContext.GroupMember
                .Include(x => x.Group)
                .FirstOrDefaultAsync(x => x.UserId == userId && x.GroupId == groupId);
            return foundEnt?.ToRuntime();
        }
        public async Task<ICollection<GroupMember>?> GetFullMemberships(Guid userId, int count, int startIndex)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var foundEnts = await dbContext.GroupMember
                .Include(x => x.Group)
                .Where(x => x.UserId == userId)
                .Skip(startIndex)
                .Take(count)
                .ToArrayAsync();
            return foundEnts?.Length > 0 ? foundEnts.Select(x => x.ToRuntime()).ToArray() : null;
        }
    }
}