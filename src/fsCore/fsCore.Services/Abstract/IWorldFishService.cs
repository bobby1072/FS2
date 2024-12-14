using fsCore.Common.Models;

namespace fsCore.Services.Abstract
{
    public interface IWorldFishService
    {
        Task<ICollection<WorldFish>> FindSomeLike(string fishAnyProperty);
        Task MigrateJsonFishToDb();
    }
}