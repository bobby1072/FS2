using System.Net;
using System.Text.Json;
using Common;
using Common.Dbinterfaces.Repository;
using Common.Models;
using Common.Utils;
using fsCore.Service.Interfaces;

namespace fsCore.Service
{
    internal class WorldFishService : BaseService<WorldFish, IWorldFishRepository>, IWorldFishService
    {
        public WorldFishService(IWorldFishRepository baseRepo) : base(baseRepo) { }
        public async Task MigrateJsonFishToDb()
        {
            var file = await File.ReadAllTextAsync(@"../Common/Data/allFish.json");
            var allFileFish = JsonSerializer.Deserialize<ICollection<JsonFileWorldFish>>(file) ?? throw new Exception();
            var allWorldFishFromFile = allFileFish.Select(x => x.ToWorldFishRegular()).ToHashSet();
            var allDbFish = await _repo.GetAll();
            if (allDbFish is null)
            {
                await _repo.Create(allWorldFishFromFile);
            }
            else
            {
                var fishToCreate = allWorldFishFromFile.Where(x => !allDbFish.Any(y => y.Taxocode == x.Taxocode)).ToArray();
                if (fishToCreate is not null && fishToCreate.Count() > 0)
                {
                    await _repo.Create(fishToCreate);
                }
            }
        }
        public async Task<ICollection<WorldFish>> FindSomeLike(string fishAnyName)
        {
            var similarFish = await _repo.FindSomeLike(fishAnyName);
            return similarFish ?? Array.Empty<WorldFish>();
        }
        public async Task<WorldFish> FindOne(string fishProp, string propertyName)
        {
            var worldFishProperties = typeof(WorldFish).GetProperties();
            var foundDetail = worldFishProperties.FirstOrDefault(x =>
            {
                var worldFishPropertyType = x.GetType();
                return x.Name == propertyName.ToPascalCase() && typeof(string) == x.PropertyType;
            }) ?? throw new ApiException(ErrorConstants.FieldNotFound, HttpStatusCode.NotFound);
            return await _repo.GetOne(fishProp, propertyName.ToPascalCase()) ?? throw new ApiException(ErrorConstants.NoFishFound, HttpStatusCode.NotFound);
        }
    }
}