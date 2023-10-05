using Common.Dbinterfaces.Repository;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;
using System.Linq;
namespace Persistence.EntityFramework.Repository
{
    internal class WorldFishRepository : BaseRepository<WorldFishEntity, WorldFish>, IWorldFishRepository
    {
        public WorldFishRepository(IDbContextFactory<FsContext> dbContextFactory) : base(dbContextFactory) { }
        public Task<ICollection<WorldFish>?> Create(ICollection<WorldFish> fishToCreate) => _create(fishToCreate.Select(x => WorldFishEntity.RuntimeToEntity(x)).ToArray());
        public Task<ICollection<WorldFish>?> Update(ICollection<WorldFish> fishToCreate) => _update(fishToCreate.Select(x => WorldFishEntity.RuntimeToEntity(x)).ToArray());
        public Task<ICollection<WorldFish>?> Delete(ICollection<WorldFish> fishToCreate) => _delete(fishToCreate.Select(x => WorldFishEntity.RuntimeToEntity(x)).ToArray());
        public async Task<ICollection<WorldFish>?> FindSomeLike(WorldFish fish)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var entity = WorldFishEntity.RuntimeToEntity(fish);
            var foundEnts = await dbContext.WorldFish
                .Where(x => EF.Functions.Like(x.Nickname.ToLower(), entity.Nickname.ToLower())
                || EF.Functions.Like(x.ScientificName.ToLower(), entity.ScientificName.ToLower())
                || EF.Functions.Like(x.EnglishName.ToLower(), entity.EnglishName.ToLower()))
                .ToArrayAsync();
            return foundEnts?.Select(x => x.ToRuntime()).ToArray();
        }
        public async Task<ICollection<WorldFish>?> FindSomeLike(string anyFish)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var foundEnts = await dbContext.WorldFish
                .Where(x => EF.Functions.Like(x.Nickname.ToLower(), anyFish.ToLower())
                || EF.Functions.Like(x.ScientificName.ToLower(), anyFish.ToLower())
                || EF.Functions.Like(x.EnglishName.ToLower(), anyFish.ToLower()))
                .ToArrayAsync();
            return foundEnts?.Select(x => x.ToRuntime()).ToArray();
        }
        public async Task<WorldFish?> GetOne(WorldFish fish)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var foundFish = await dbContext.WorldFish.FirstOrDefaultAsync(x => WorldFishEntity.RuntimeToEntity(fish).Taxocode == x.Taxocode);
            return foundFish?.ToRuntime();
        }
    }
}