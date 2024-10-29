using Common.Models;

namespace Services.Abstract
{
    public interface IWorldFishService
    {
        Task<ICollection<WorldFish>> FindSomeLike(string fishAnyProperty);
        Task MigrateJsonFishToDb();
    }
}