using Common.Models;

namespace fsCore.Service.Interfaces
{
    public interface IWorldFishService
    {
        Task<ICollection<WorldFish>> AllFish();
        Task<ICollection<WorldFish>> FindSomeLike(WorldFish fishAny);
        Task<ICollection<WorldFish>> FindSomeLike(string fishAnyProperty);
        Task<WorldFish> FindOne(WorldFish fish);
        Task<WorldFish> FindOne(string fishProp, string propertyName);
        Task<WorldFish?> CreateFish(WorldFish newFish, bool includeFish = false);
        Task MigrateJsonFishToDb();
    }
}