using Common.Models;

namespace fsCore.Service.Interfaces
{
    public interface IWorldFishService
    {
        Task<ICollection<WorldFish>> FindSomeLike(string fishAnyProperty);
        Task<WorldFish> FindOne(string fishProp, string propertyName);
        Task MigrateJsonFishToDb();
    }
}