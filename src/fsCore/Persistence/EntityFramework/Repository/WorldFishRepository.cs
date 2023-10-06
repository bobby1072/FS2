using Common.Dbinterfaces.Repository;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;
using System.Linq;
namespace Persistence.EntityFramework.Repository
{
    internal class WorldFishRepository : BaseRepository<WorldFishEntity>, IWorldFishRepository
    {
        public WorldFishRepository(IDbContextFactory<FsContext> dbContextFactory) : base(dbContextFactory) { }
        public async Task<ICollection<WorldFish>?> GetAll()
        {
            var allBaseModel = await _getAll();
            return allBaseModel?.Select(x => x as WorldFish).ToList();
        }
        public async Task<WorldFish?> GetOne<TField>(TField field, string fieldName)
        {
            var baseModel = await _getOne(field, fieldName);
            return baseModel as WorldFish;
        }
        public async Task<ICollection<WorldFish>?> GetSomeLike<TField>(TField field, string fieldName)
        {
            var baseModel = await _getSomeLike(field, fieldName);
            return baseModel?.Select(x => x as WorldFish).ToList();
        }
        public async Task<ICollection<WorldFish>?> Create(ICollection<WorldFish> fishToCreate)
        {
            var localModels = await _create(fishToCreate.Select(x => WorldFishEntity.RuntimeToEntity(x)).ToList());
            return localModels?.Select(x => x as WorldFish).ToList();
        }
        public async Task<ICollection<WorldFish>?> Update(ICollection<WorldFish> fishToCreate)
        {
            var localModels = await _update(fishToCreate.Select(x => WorldFishEntity.RuntimeToEntity(x)).ToList());
            return localModels?.Select(x => x as WorldFish).ToList();
        }
        public async Task<ICollection<WorldFish>?> Delete(ICollection<WorldFish> fishToCreate)
        {
            var localModels = await _delete(fishToCreate.Select(x => WorldFishEntity.RuntimeToEntity(x)).ToList());
            return localModels?.Select(x => x as WorldFish).ToList();
        }
        public async Task<ICollection<WorldFish>?> FindSomeLike(WorldFish fish)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var entity = WorldFishEntity.RuntimeToEntity(fish);
            var foundEnts = await dbContext.WorldFish
                .Where(x => EF.Functions.Like(x.Nickname.ToLower(), entity.Nickname.ToLower())
                || EF.Functions.Like(x.ScientificName.ToLower(), entity.ScientificName.ToLower())
                || EF.Functions.Like(x.EnglishName.ToLower(), entity.EnglishName.ToLower()))
                .ToListAsync();
            return foundEnts?.Select(x => x.ToRuntime()).ToList();
        }
        public async Task<ICollection<WorldFish>?> FindSomeLike(string anyFish)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var foundEnts = await dbContext.WorldFish
                .Where(x => EF.Functions.Like(x.Nickname.ToLower(), anyFish.ToLower())
                || EF.Functions.Like(x.ScientificName.ToLower(), anyFish.ToLower())
                || EF.Functions.Like(x.EnglishName.ToLower(), anyFish.ToLower()))
                .ToListAsync();
            return foundEnts?.Select(x => x.ToRuntime()).ToList();
        }
        public async Task<WorldFish?> GetOne(WorldFish fish)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var foundFish = await dbContext.WorldFish.FirstOrDefaultAsync(x => WorldFishEntity.RuntimeToEntity(fish).Taxocode == x.Taxocode);
            return foundFish?.ToRuntime();
        }
    }
}