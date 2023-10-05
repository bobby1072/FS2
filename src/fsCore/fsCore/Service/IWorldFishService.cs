using Common.Models;

namespace fsCore.Service
{
    public interface IWorldFishService
    {
        Task<ICollection<WorldFish>> AllFish();
        Task<ICollection<WorldFish>> FindSomeLike(WorldFish fishAny);
        Task<ICollection<WorldFish>> FindSomeLike(string fishAnyName);
        Task<WorldFish> FindOne(WorldFish fish);
        Task<WorldFish> FindOne(string fishName);
        Task<WorldFish> GetFullFish(WorldFish fish);
    }
}