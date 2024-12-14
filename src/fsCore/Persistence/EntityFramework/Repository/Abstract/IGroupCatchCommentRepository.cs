using fsCore.Common.Models;
using Persistence.EntityFramework.Repository.Concrete;

namespace Persistence.EntityFramework.Repository.Abstract
{
    public interface IGroupCatchCommentRepository
    {
        Task<ICollection<GroupCatchComment>?> Create(ICollection<GroupCatchComment> GroupCatchCommentToCreate);
        Task<ICollection<GroupCatchComment>?> Update(ICollection<GroupCatchComment> GroupCatchCommentToUpdate);
        Task<ICollection<GroupCatchComment>?> Delete(ICollection<GroupCatchComment> GroupCatchCommentToDelete);
        Task<ICollection<GroupCatchComment>?> GetAllForCatch(Guid catchId);
        Task<GroupCatchComment?> GetOne(int id);

        Task<GroupCatchComment?> SaveFullGroupCatchComment(GroupCatchComment groupCatchComment,
            ICollection<GroupCatchCommentTaggedUser> users,
            SaveFullGroupCatchCommentType saveFullGroupCatchCommentType);
        Task<ICollection<GroupCatchCommentTaggedUser>?> DeleteTaggedUsers(ICollection<int> commentIds);
        Task<ICollection<GroupCatchCommentTaggedUser>?> CreateTaggedUsers(ICollection<GroupCatchCommentTaggedUser> GroupCatchCommentTaggedUsersToCreate);
    }
}