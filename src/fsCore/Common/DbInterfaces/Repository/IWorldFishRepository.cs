using Common.Models;

namespace Common.Dbinterfaces.Repository
{
    public interface IWorldFishRepository
    {
        Task<ICollection<WorldFish>?> Create(ICollection<WorldFish> WorldFishToCreate);
        Task<ICollection<WorldFish>?> Delete(ICollection<WorldFish> WorldFishToDelete);
        Task<ICollection<WorldFish>?> GetAll(ICollection<string>? relationships = null);
        Task<WorldFish?> GetOne<T>(T field, string fieldName, ICollection<string>? relationships = null);
        Task<WorldFish?> GetOne(WorldFish WorldFish, ICollection<string>? relationships = null);
        Task<ICollection<WorldFish>?> FindSomeLike(string anyFish);
        Task<ICollection<WorldFish>?> FindSomeLike(WorldFish fish);
    }
}