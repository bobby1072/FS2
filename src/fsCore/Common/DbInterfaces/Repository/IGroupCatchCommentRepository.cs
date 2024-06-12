using Common.Models;

namespace Common.DbInterfaces.Repository
{
    public interface IGroupCatchCommentRepository
    {
        Task<ICollection<GroupCatchComment>?> Create(ICollection<GroupCatchComment> GroupCatchCommentToCreate);
        Task<ICollection<GroupCatchComment>?> Update(ICollection<GroupCatchComment> GroupCatchCommentToUpdate);
        Task<ICollection<GroupCatchComment>?> Delete(ICollection<GroupCatchComment> GroupCatchCommentToDelete);
        Task<ICollection<GroupCatchComment>?> GetAllForCatch(Guid catchId);
        Task<GroupCatchComment?> GetOne(int id);
        Task<ICollection<GroupCatchCommentTaggedUsers>?> DeleteTaggedUsers(ICollection<int> commentIds);
        Task<ICollection<GroupCatchCommentTaggedUsers>?> CreateTaggedUsers(ICollection<GroupCatchCommentTaggedUsers> GroupCatchCommentTaggedUsersToCreate);
    }
}