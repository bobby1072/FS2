using Common;
using Common.Dbinterfaces.Repository;
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
        public async Task<GroupMember?> GetGroupMemberIncludingUser(string userEmail, Guid groupId)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var foundGroupMember = await dbContext.GroupMember
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserEmail == userEmail && x.GroupId == groupId);
            return foundGroupMember?.ToRuntime();
        }
        public async Task<GroupMember?> GetGroupMemberIncludingUserAndGroup(string userEmail, Guid groupId)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var foundGroupMember = await dbContext.GroupMember
                .Include(x => x.User)
                .Include(x => x.Group)
                .FirstOrDefaultAsync(x => x.UserEmail == userEmail && x.GroupId == groupId);
            return foundGroupMember?.ToRuntime();
        }
        public async Task<GroupMember?> GetGroupMemberIncludingPosition(string userEmail, Guid groupId)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var foundGroupMember = await dbContext.GroupMember
                .Include(x => x.Position)
                .FirstOrDefaultAsync(x => x.UserEmail == userEmail && x.GroupId == groupId);
            return foundGroupMember?.ToRuntime();
        }
        public async Task<ICollection<GroupMember>?> GetManyGroupMembersIncludingUser(Guid groupId)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var foundGroupMembers = await dbContext.GroupMember
                .Include(x => x.User)
                .Where(x => x.GroupId == groupId)
                .ToArrayAsync();
            var runtimeArray = foundGroupMembers?.Select(x => x.ToRuntime());
            return runtimeArray?.OfType<GroupMember>().ToList();
        }
        public async Task<ICollection<GroupMember>?> GetManyGroupMemberForUserIncludingUserAndPositionAndGroup(string userEmail)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var foundGroupMembers = await dbContext.GroupMember
                .Include(x => x.User)
                .Include(x => x.Group)
                .Include(x => x.Position)
                .Where(x => x.UserEmail == userEmail)
                .ToArrayAsync();
            var runtimeArray = foundGroupMembers?.Select(x => x.ToRuntime());
            return runtimeArray?.OfType<GroupMember>().ToList();
        }
    }
}