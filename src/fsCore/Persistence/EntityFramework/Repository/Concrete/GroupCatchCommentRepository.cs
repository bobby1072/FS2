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


        public async Task<GroupCatchComment?> SaveFullGroupCatchComment(GroupCatchComment groupCatchComment,
            ICollection<GroupCatchCommentTaggedUser> users, SaveFullGroupCatchCommentType saveFullGroupCatchCommentType)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            await using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                var groupCatchCommentEntity = GroupCatchCommentEntity.RuntimeToEntity(groupCatchComment);
                
                
                Func<Task> commentJob = saveFullGroupCatchCommentType == SaveFullGroupCatchCommentType.Create ? () => dbContext.GroupCatchComment.AddAsync(groupCatchCommentEntity).AsTask()
                        : () => Task.FromResult(dbContext.GroupCatchComment.Update(groupCatchCommentEntity));
                
                await commentJob();
                await dbContext.SaveChangesAsync();
                
                var newComment = dbContext.GroupCatchComment.Local.First();
                if (users.Count > 0)
                {
                    var taggedEntities = users.Select(x =>
                    {
                        var ent = GroupCatchCommentTaggedUsersEntity.RuntimeToEntity(x);
                        ent.CommentId = newComment.Id;
                        return ent;
                    }).ToArray();

                    await dbContext.GroupCatchCommentTaggedUsers.AddRangeAsync(taggedEntities);
                        
                    await dbContext.SaveChangesAsync();
                }
                
                await transaction.CommitAsync();
                
                return newComment.ToRuntime();
                
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<ICollection<GroupCatchComment>?> GetAllForCatch(Guid catchId)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
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
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var comment = await dbContext.GroupCatchComment
                .Include(c => c.User)
                .Include(c => c.TaggedUsers)!
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
            return comment?.ToRuntime();
        }
        public async Task<ICollection<GroupCatchCommentTaggedUser>?> DeleteTaggedUsers(ICollection<int> commentIds)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var foundEntities = await context.GroupCatchCommentTaggedUsers.Where(x => commentIds.Contains(x.CommentId)).ToArrayAsync();
            context.GroupCatchCommentTaggedUsers.RemoveRange(foundEntities);
            await context.SaveChangesAsync();
            return foundEntities.Select(x => x.ToRuntime()).ToArray();
        }
        public async Task<ICollection<GroupCatchCommentTaggedUser>?> CreateTaggedUsers(ICollection<GroupCatchCommentTaggedUser> GroupCatchCommentTaggedUsersToCreate)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var entities = GroupCatchCommentTaggedUsersToCreate.Select(x => GroupCatchCommentTaggedUsersEntity.RuntimeToEntity(x)).ToArray();
            await context.GroupCatchCommentTaggedUsers.AddRangeAsync(entities);
            await context.SaveChangesAsync();
            return context.GroupCatchCommentTaggedUsers.Local.Select(x => x.ToRuntime()).ToArray();
        }
    }
    public enum SaveFullGroupCatchCommentType
    {
        Update, Create
    }
}