using Common;
using Common.DbInterfaces.Repository;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;

namespace Persistence.EntityFramework.Repository
{
    internal class GroupMemberRepository : BaseRepository<GroupMemberEntity, GroupMember>, IGroupMemberRepository
    {
        public GroupMemberRepository(IDbContextFactory<FsContext> context) : base(context) { }
        protected override GroupMemberEntity _runtimeToEntity(GroupMember runtimeObj)
        {
            return GroupMemberEntity.RuntimeToEntity(runtimeObj);
        }
        public async Task<ICollection<GroupMember>?> GetFullMemberships(Guid userId, int count, int startIndex)
        {
            await using var dbContext = await DbContextFactory.CreateDbContextAsync();
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