using Common.Models;

namespace Common.Dbinterfaces.Repository
{
    public interface IGroupMemberRepository
    {
        Task<ICollection<GroupMember>?> GetAll();
        Task<ICollection<GroupMember>?> Create(ICollection<GroupMember> groupMemberToCreate);
        Task<ICollection<GroupMember>?> Update(ICollection<GroupMember> groupMemberToUpdate);
        Task<ICollection<GroupMember>?> Delete(ICollection<GroupMember> groupMemberToDelete);
        Task<GroupMember?> GetOne<T>(T field, string fieldName);
        Task<GroupMember?> GetOne(GroupMember groupMember);
    }
}