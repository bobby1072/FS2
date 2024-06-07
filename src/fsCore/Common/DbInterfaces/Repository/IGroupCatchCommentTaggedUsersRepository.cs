using Common.Models;

namespace Common.DbInterfaces.Repository
{
    public interface IGroupCatchCommentTaggedUsersRepository
    {
        Task<ICollection<GroupCatchCommentTaggedUsers>?> Create(ICollection<GroupCatchCommentTaggedUsers> GroupCatchCommentTaggedUsersToCreate);
        Task<ICollection<GroupCatchCommentTaggedUsers>?> Delete(ICollection<GroupCatchCommentTaggedUsers> GroupCatchCommentTaggedUsersToDelete);
    }
}