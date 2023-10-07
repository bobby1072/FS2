using System.Net;
using System.Text.Json;
using Common;
using Common.Dbinterfaces.Repository;
using Common.Models;

namespace fsCore.Service
{
    public class WorldFishService : BaseService<WorldFish, IWorldFishRepository>, IWorldFishService
    {
        public WorldFishService(IWorldFishRepository baseRepo) : base(baseRepo) { }
        public async Task MigrateJsonFishToDb()
        {
            var file = File.ReadAllText(@"../Common/Data/allFish.json");
            var allFileFish = JsonSerializer.Deserialize<ICollection<JsonFileWorldFish>>(file) ?? throw new Exception();
            var allWorldFishFromFile = allFileFish.Select(x => x.ToWorldFishRegular()).ToList();
            var allDbFish = await _repo.GetAll();
            if (allDbFish is null)
            {
                await _repo.Create(allWorldFishFromFile);
            }
            else
            {
                var fishToCreate = allWorldFishFromFile.Where(x => !allDbFish.Any(y => y.Taxocode == x.Taxocode)).ToList();
                if (fishToCreate is not null && fishToCreate.Count > 0)
                {
                    await _repo.Create(fishToCreate);
                }
            }
        }
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
        public async Task<WorldFish?> CreateFish(WorldFish newFish, bool includeFish = false)
        {
            var foundCopy = await _repo.GetOne(newFish);
            if (foundCopy is not null)
            {
                throw new ApiException(ErrorConstants.FishAlreadyExists, HttpStatusCode.Conflict);
            }
            var createdFish = await _repo.Create(new List<WorldFish> { newFish }) ?? throw new ApiException(ErrorConstants.FailedToCreateFish);
            return includeFish ? createdFish.FirstOrDefault() : null;
        }
    }
}