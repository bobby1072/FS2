using Common.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;
using Persistence.EntityFramework.Repository.Abstract;

namespace Persistence.EntityFramework.Repository.Concrete
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
                .Include(x => x.Catches)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            return entity?.ToRuntime();
        }
    }
}