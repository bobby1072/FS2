using Common.Dbinterfaces.Repository;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;

namespace Persistence.EntityFramework.Repository
{
    internal class WorldFishRepository : BaseRepository<WorldFishEntity, WorldFish>, IWorldFishRepository
    {
        public WorldFishRepository(IDbContextFactory<FsContext> dbContextFactory) : base(dbContextFactory) { }
        public Task<ICollection<WorldFish>?> Create(ICollection<WorldFish> fishToCreate) => _create(fishToCreate.Select(x => WorldFishEntity.RuntimeToEntity(x)).ToArray());
        public Task<ICollection<WorldFish>?> Update(ICollection<WorldFish> fishToCreate) => _update(fishToCreate.Select(x => WorldFishEntity.RuntimeToEntity(x)).ToArray());
        public Task<ICollection<WorldFish>?> Delete(ICollection<WorldFish> fishToCreate) => _delete(fishToCreate.Select(x => WorldFishEntity.RuntimeToEntity(x)).ToArray());
    }
}