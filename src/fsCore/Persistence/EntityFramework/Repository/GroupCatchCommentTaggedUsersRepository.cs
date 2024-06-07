using Common.DbInterfaces.Repository;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;

namespace Persistence.EntityFramework.Repository
{
    internal class GroupCatchCommentTaggedUsersRepository : BaseRepository<GroupCatchCommentTaggedUsersEntity, GroupCatchCommentTaggedUsers>, IGroupCatchCommentTaggedUsersRepository
    {
        public GroupCatchCommentTaggedUsersRepository(IDbContextFactory<FsContext> dbContextFactory) : base(dbContextFactory) { }
        protected override GroupCatchCommentTaggedUsersEntity RuntimeToEntity(GroupCatchCommentTaggedUsers runtimeObj)
        {
            return GroupCatchCommentTaggedUsersEntity.RuntimeToEntity(runtimeObj);
        }
        public async Task<ICollection<GroupCatchCommentTaggedUsers>?> Delete(ICollection<int> commentIds)
        {
            await using var context = await DbContextFactory.CreateDbContextAsync();
            var foundEntities = await context.CommentTaggedUsers.Where(x => commentIds.Contains(x.CommentId)).ToArrayAsync();
            context.CommentTaggedUsers.RemoveRange(foundEntities);
            await context.SaveChangesAsync();
            return foundEntities.Select(x => x.ToRuntime()).ToArray();
        }
    }
}