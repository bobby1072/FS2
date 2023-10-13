using Common.Models;

namespace Common.Dbinterfaces.Repository
{
    public interface IGroupRepository
    {
        Task<ICollection<GroupModel>?> GetAll();
        Task<ICollection<GroupModel>?> Create(ICollection<GroupModel> GroupModelToCreate);
        Task<ICollection<GroupModel>?> Update(ICollection<GroupModel> fishToUpdate);
        Task<ICollection<GroupModel>?> Delete(ICollection<GroupModel> fishToDelete);
        Task<GroupModel?> GetOne<T>(T field, string fieldName);
        Task<GroupModel?> GetOne(GroupModel GroupModel);
    }
}