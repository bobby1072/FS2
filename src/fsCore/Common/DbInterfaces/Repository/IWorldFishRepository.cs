using Common.Models;

namespace Common.Dbinterfaces.Repository
{
    public interface IWorldFishRepository
    {
        Task<ICollection<WorldFish>> GetAll();
        Task<ICollection<WorldFish>?> Create(ICollection<WorldFish> fishToCreate);
        Task<WorldFish?> GetOne<T>(T field, string fieldName);
        Task<WorldFish?> GetOne(WorldFish fish);
        Task<ICollection<WorldFish>?> Update(ICollection<WorldFish> fishToUpdate);
        Task<ICollection<WorldFish>?> Delete(ICollection<WorldFish> fishToDelete);
        Task<ICollection<WorldFish>?> FindSomeLike(string anyFish);
        Task<ICollection<WorldFish>?> FindSomeLike(WorldFish fish);
    }
}