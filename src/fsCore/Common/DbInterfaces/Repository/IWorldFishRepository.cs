using Common.Models;

namespace Common.Dbinterfaces.Repository
{
    public interface IWorldFishRepository
    {
        Task<ICollection<WorldFish>> GetAll();
        Task<ICollection<WorldFish>?> Create(ICollection<WorldFish> fishToCreate);
        Task<WorldFish?> GetOne<T>(T field, string fieldName);
        Task<ICollection<WorldFish>?> Update(ICollection<WorldFish> fishToUpdate);
        Task<ICollection<WorldFish>?> Delete(ICollection<WorldFish> fishToDelete);
    }
}