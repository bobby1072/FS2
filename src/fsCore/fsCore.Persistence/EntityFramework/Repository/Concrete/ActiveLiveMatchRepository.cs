using fsCore.Common.Models;
using fsCore.Persistence.EntityFramework;
using fsCore.Persistence.EntityFramework.Entity;
using fsCore.Persistence.EntityFramework.Repository.Abstract;
using Microsoft.EntityFrameworkCore;

namespace fsCore.Persistence.EntityFramework.Repository.Concrete
{
    internal class ActiveLiveMatchRepository : BaseRepository<ActiveLiveMatchEntity, LiveMatch>, IActiveLiveMatchRepository
    {
        public ActiveLiveMatchRepository(IDbContextFactory<FsContext> context) : base(context) { }
        protected override ActiveLiveMatchEntity RuntimeToEntity(LiveMatch runtimeObj)
        {
            return ActiveLiveMatchEntity.FromRuntime(runtimeObj);
        }
        public async Task<LiveMatch?> GetFullOneById(Guid id)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();

            var entity = await dbContext
                .ActiveLiveMatch
                .Include(x => x.Participants)!
                .ThenInclude(x => x.User)
                .Include(x => x.Catches)!
                .ThenInclude(x => x.WorldFish)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            return entity?.ToRuntime();
        }
        public async Task<ICollection<LiveMatch>?> GetFullOneById(ICollection<Guid> ids)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();

            var entity = await dbContext
                .ActiveLiveMatch
                .Include(x => x.Participants)!
                .ThenInclude(x => x.User)
                .Include(x => x.Catches)!
                .ThenInclude(x => x.WorldFish)
                .Where(x => ids.Contains(x.Id))
                .Take(10)
                .ToArrayAsync();

            return entity?.Select(x => x.ToRuntime()).ToArray();
        }
    }
}