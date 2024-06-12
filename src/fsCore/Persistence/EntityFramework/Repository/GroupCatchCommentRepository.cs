using Common.DbInterfaces.Repository;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;

namespace Persistence.EntityFramework.Repository
{
    internal class GroupCatchCommentRepository : BaseRepository<GroupCatchCommentEntity, GroupCatchComment>, IGroupCatchCommentRepository
    {
        public GroupCatchCommentRepository(IDbContextFactory<FsContext> dbContext) : base(dbContext)
        {
        }
        protected override GroupCatchCommentEntity RuntimeToEntity(GroupCatchComment runtimeObj)
        {
            return GroupCatchCommentEntity.RuntimeToEntity(runtimeObj);
        }
        public async Task<ICollection<GroupCatchComment>?> GetAllForCatch(Guid catchId)
        {
            await using var dbContext = await DbContextFactory.CreateDbContextAsync();
            var comments = await dbContext.GroupCatchComment
                .Include(c => c.User)
                .Include(c => c.TaggedUsers)!
                .ThenInclude(c => c.User)
                .Where(c => c.CatchId == catchId)
                .ToArrayAsync();
            return comments?.Select(x => x.ToRuntime()).ToArray();
        }
        public async Task<GroupCatchComment?> GetOne(int id)
        {
            await using var dbContext = await DbContextFactory.CreateDbContextAsync();
            var comment = await dbContext.GroupCatchComment
                .Include(c => c.User)
                .Include(c => c.TaggedUsers)!
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
            return comment?.ToRuntime();
        }
    }
}