using Common.Models;

namespace fsCore.Service
{
    public interface IWorldFishService
    {
        Task<ICollection<WorldFish>> AllFish();
        Task<ICollection<WorldFish>> FindSomeLike(WorldFish fishAny);
        Task<ICollection<WorldFish>> FindSomeLike(string fishAnyProperty);
        Task<WorldFish> FindOne(WorldFish fish);
        Task<WorldFish> FindOne(string fishProp, string propertyName);
        Task<WorldFish> FindOne(int isscaap);
        //Task<WorldFish> GetFullFish(WorldFish fish);
    }
}