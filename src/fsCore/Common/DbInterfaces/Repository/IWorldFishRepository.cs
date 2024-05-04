using Common.Models;

namespace Common.DbInterfaces.Repository
{
    public interface IWorldFishRepository
    {
        Task DeleteAll();
        Task<ICollection<WorldFish>?> Create(ICollection<WorldFish> WorldFishToCreate);
        Task<ICollection<WorldFish>?> Delete(ICollection<WorldFish> WorldFishToDelete);
        Task<ICollection<WorldFish>?> GetAll(params string[] relationships);
        Task<WorldFish?> GetOne<T>(T field, string fieldName, ICollection<string>? relationships = null);
        Task<WorldFish?> GetOne(WorldFish WorldFish, ICollection<string>? relationships = null);
        Task<ICollection<WorldFish>?> FindSomeLike(string anyFish);
    }
}