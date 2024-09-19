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
        public async Task<ICollection<Guid>> GetForUser(Guid userId)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();

            var entities = await dbContext
                .ActiveLiveMatchParticipant
                .Where(x => x.UserId == userId)
                .Select(x => x.MatchId)
                .ToArrayAsync();

            return entities;
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

        // public async Task<ICollection<LiveMatch>?> GetMatchesForUser( LiveMatchStatus status)
        // {
        //     await using var dbContext = await _contextFactory.CreateDbContextAsync();

        //     var entity = await dbContext
        //         .ActiveLiveMatch
        //         .Where(x => x.Id == id)
        //         .FirstOrDefaultAsync();

        //     return entity?.ToRuntime();
        // }
    }
}