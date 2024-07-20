using Common.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;
using Persistence.EntityFramework.Repository.Abstract;
namespace Persistence.EntityFramework.Repository.Concrete
{
    internal class WorldFishRepository : BaseRepository<WorldFishEntity, WorldFish>, IWorldFishRepository
    {
        protected override WorldFishEntity RuntimeToEntity(WorldFish runtimeObj)
        {
            return WorldFishEntity.RuntimeToEntity(runtimeObj);
        }
        public WorldFishRepository(IDbContextFactory<FsContext> dbContextFactory) : base(dbContextFactory) { }
        public async Task<ICollection<WorldFish>?> FindSomeLike(string anyFish)
        {
            await using var dbContext = await DbContextFactory.CreateDbContextAsync();
            var foundEnts = await dbContext.WorldFish
                .Where(x => x.Nickname != null && x.Nickname.ToLower().Contains(anyFish.ToLower()) ||
                            x.ScientificName != null && x.ScientificName.ToLower().Contains(anyFish.ToLower()) ||
                            x.EnglishName != null && x.EnglishName.ToLower().Contains(anyFish.ToLower()))
                .Take(30)
                .ToArrayAsync();
            return foundEnts?.Select(x => x.ToRuntime()).ToArray();
        }
    }
}