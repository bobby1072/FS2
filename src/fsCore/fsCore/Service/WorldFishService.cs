using System.Net;
using Common;
using Common.Dbinterfaces.Repository;
using Common.Models;

namespace fsCore.Service
{
    public class WorldFishService : BaseService<WorldFish, IWorldFishRepository>, IWorldFishService
    {
        public WorldFishService(IWorldFishRepository baseRepo) : base(baseRepo) { }
        public async Task<ICollection<WorldFish>> AllFish()
        {
            var allFish = await _repo.GetAll();
            return allFish ?? throw new ApiException(ErrorConstants.NoFishFound, HttpStatusCode.NotFound);
        }
        public async Task<ICollection<WorldFish>> FindSomeLike(WorldFish fishAny)
        {
            var similarFish = await _repo.FindSomeLike(fishAny);
            return similarFish ?? throw new ApiException(ErrorConstants.NoFishFound, HttpStatusCode.NotFound);
        }
        public async Task<ICollection<WorldFish>> FindSomeLike(string fishAnyName)
        {
            var similarFish = await _repo.FindSomeLike(fishAnyName);
            return similarFish ?? throw new ApiException(ErrorConstants.NoFishFound, HttpStatusCode.NotFound);
        }
        public async Task<WorldFish> FindOne(WorldFish fish)
        {
            var foundFish = await _repo.GetOne(fish);
            return foundFish ?? throw new ApiException(ErrorConstants.NoFishFound, HttpStatusCode.NotFound);
        }
        public async Task<WorldFish> FindOne(string fishProp, string propertyName)
        {
            var worldFishProperties = typeof(WorldFish).GetProperties();
            var foundDetail = worldFishProperties.FirstOrDefault(x =>
            {
                var worldFishPropertyType = x.GetType();
                return x.Name == propertyName && typeof(string) == x.PropertyType;
            }) ?? throw new ApiException(ErrorConstants.FieldNotFound, HttpStatusCode.NotFound);
            return await _repo.GetOne(fishProp, propertyName) ?? throw new ApiException(ErrorConstants.NoFishFound, HttpStatusCode.NotFound);
        }
        public async Task<WorldFish> FindOne(int isscaap)
        {
            return await _repo.GetOne(isscaap, typeof(WorldFish).GetProperty("Isscaap")?.Name ?? throw new Exception()) ?? throw new ApiException(ErrorConstants.NoFishFound, HttpStatusCode.NotFound);
        }
    }
}