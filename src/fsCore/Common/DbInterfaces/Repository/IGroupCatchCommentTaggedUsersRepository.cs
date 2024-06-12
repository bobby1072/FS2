using Common.Models;

namespace Common.DbInterfaces.Repository
{
    public interface IGroupCatchCommentTaggedUsersRepository
    {
        Task<ICollection<GroupCatchCommentTaggedUsers>?> Delete(ICollection<int> commentIds);
        Task<ICollection<GroupCatchCommentTaggedUsers>?> Create(ICollection<GroupCatchCommentTaggedUsers> GroupCatchCommentTaggedUsersToCreate);
    }
}