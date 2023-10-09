using Common.Dbinterfaces.Repository;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;
namespace Persistence.EntityFramework.Repository
{
    internal class WorldFishRepository : BaseRepository<WorldFishEntity, WorldFish>, IWorldFishRepository
    {
        public WorldFishRepository(IDbContextFactory<FsContext> dbContextFactory) : base(dbContextFactory) { }
        public Task<ICollection<WorldFish>?> Create(ICollection<WorldFish> fishToCreate) => _create(fishToCreate.Select(x => WorldFishEntity.RuntimeToEntity(x)).ToList());
        public Task<ICollection<WorldFish>?> Update(ICollection<WorldFish> fishToCreate) => _update(fishToCreate.Select(x => WorldFishEntity.RuntimeToEntity(x)).ToList());
        public Task<ICollection<WorldFish>?> Delete(ICollection<WorldFish> fishToCreate) => _delete(fishToCreate.Select(x => WorldFishEntity.RuntimeToEntity(x)).ToList());
        public async Task<ICollection<WorldFish>?> FindSomeLike(WorldFish fish)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var entity = WorldFishEntity.RuntimeToEntity(fish);
            var foundEnts = await dbContext.WorldFish
                .Where(x => (fish.Nickname != null && x.Nickname != null && x.Nickname.ToLower().Contains(fish.Nickname.ToLower())) ||
                            (fish.ScientificName != null && x.ScientificName != null && x.ScientificName.ToLower().Contains(fish.ScientificName.ToLower())) ||
                            (fish.EnglishName != null && x.EnglishName != null && x.EnglishName.ToLower().Contains(fish.EnglishName.ToLower())))
                .ToListAsync();
            return foundEnts?.Select(x => x.ToRuntime()).ToList();
        }
        public async Task<ICollection<WorldFish>?> FindSomeLike(string anyFish)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var foundEnts = await dbContext.WorldFish
                .Where(x => (x.Nickname != null && x.Nickname.ToLower().Contains(anyFish.ToLower())) ||
                            (x.ScientificName != null && x.ScientificName.ToLower().Contains(anyFish.ToLower())) ||
                            (x.EnglishName != null && x.EnglishName.ToLower().Contains(anyFish.ToLower())))
                .ToListAsync();
            return foundEnts?.Select(x => x.ToRuntime()).ToList();
        }
    }
}