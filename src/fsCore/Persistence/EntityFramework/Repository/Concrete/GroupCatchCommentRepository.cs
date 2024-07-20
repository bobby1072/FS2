using Common.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;
using Persistence.EntityFramework.Repository.Abstract;

namespace Persistence.EntityFramework.Repository.Concrete
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
                .OrderBy(c => c.CreatedAt)
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
        public async Task<ICollection<GroupCatchCommentTaggedUsers>?> DeleteTaggedUsers(ICollection<int> commentIds)
        {
            await using var context = await DbContextFactory.CreateDbContextAsync();
            var foundEntities = await context.CommentTaggedUsers.Where(x => commentIds.Contains(x.CommentId)).ToArrayAsync();
            context.CommentTaggedUsers.RemoveRange(foundEntities);
            await context.SaveChangesAsync();
            return foundEntities.Select(x => x.ToRuntime()).ToArray();
        }
        public async Task<ICollection<GroupCatchCommentTaggedUsers>?> CreateTaggedUsers(ICollection<GroupCatchCommentTaggedUsers> GroupCatchCommentTaggedUsersToCreate)
        {
            await using var context = await DbContextFactory.CreateDbContextAsync();
            var entities = GroupCatchCommentTaggedUsersToCreate.Select(x => GroupCatchCommentTaggedUsersEntity.RuntimeToEntity(x)).ToArray();
            await context.CommentTaggedUsers.AddRangeAsync(entities);
            await context.SaveChangesAsync();
            return context.CommentTaggedUsers.Local.Select(x => x.ToRuntime()).ToArray();
        }
    }
}