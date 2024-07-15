using Common.Models;

namespace fsCore.Service.Abstract
{
    public interface IWorldFishService
    {
        Task<ICollection<WorldFish>> FindSomeLike(string fishAnyProperty);
        Task<WorldFish> FindOne(string fishProp, string propertyName);
        Task MigrateJsonFishToDb();
    }
}