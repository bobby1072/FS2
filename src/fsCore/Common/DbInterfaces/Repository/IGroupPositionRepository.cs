using Common.Models;

namespace Common.Dbinterfaces.Repository
{
    public interface IGroupPositionRepository
    {
        Task<ICollection<GroupPosition>?> GetAll();
        Task<ICollection<GroupPosition>?> Create(ICollection<GroupPosition> positionToCreate);
        Task<ICollection<GroupPosition>?> Update(ICollection<GroupPosition> positionToUpdate);
        Task<ICollection<GroupPosition>?> Delete(ICollection<GroupPosition> positionToDelete);
        Task<GroupPosition?> GetOne<T>(T field, string fieldName);
        Task<GroupPosition?> GetOne(GroupPosition position);
    }
}