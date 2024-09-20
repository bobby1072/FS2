using Common.Models;
using Common.Utils;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;
using Persistence.EntityFramework.Repository.Abstract;

namespace Persistence.EntityFramework.Repository.Concrete
{
    internal class ActiveLiveMatchParticipantRepository : IActiveLiveMatchParticipantRepository
    {
        private readonly IDbContextFactory<FsContext> _contextFactory;
        public ActiveLiveMatchParticipantRepository(IDbContextFactory<FsContext> context)
        {
            _contextFactory = context;
        }
        public async Task<ICollection<Guid>> GetMatchIdsForUser(Guid userId)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();

            var entities = await dbContext
                .ActiveLiveMatchParticipant
                .Where(x => x.UserId == userId)
                .Select(x => x.MatchId)
                .ToArrayAsync();

            return entities;
        }
        public async Task<ICollection<LiveMatchParticipant>?> GetForMatch(Guid matchId)
        {
            await using var context = _contextFactory.CreateDbContext();
            var entities = await context.ActiveLiveMatchParticipant.Where(x => x.MatchId == matchId).ToArrayAsync();
            var returnObjs = entities.SelectWhere(x => x.User is not null, x => x.ToRuntime()).ToArray();
            return returnObjs.Length > 0 ? (ICollection<LiveMatchParticipant>)returnObjs! : null;
        }
        public async Task<ICollection<LiveMatchParticipant>?> Create(ICollection<LiveMatchParticipant> runtimeObjs, Guid matchId)
        {
            await using var context = _contextFactory.CreateDbContext();
            var entities = runtimeObjs.Select(x => ActiveLiveMatchParticipantEntity.FromRuntime(x, matchId)).ToArray();
            await context.ActiveLiveMatchParticipant.AddRangeAsync(entities);
            await context.SaveChangesAsync();
            var returnObjs = context.ActiveLiveMatchParticipant.Local.SelectWhere(x => x.User is not null, x => x.ToRuntime()).ToArray();
            return returnObjs.Length > 0 ? (ICollection<LiveMatchParticipant>)returnObjs! : null;
        }
        public async Task<ICollection<LiveMatchParticipant>?> Update(ICollection<LiveMatchParticipant> newRuntimeObjs, Guid matchId)
        {
            await using var context = _contextFactory.CreateDbContext();
            var userIdList = newRuntimeObjs.Select(x => x.Id).ToArray();
            var entities = await context.ActiveLiveMatchParticipant.Where(x => x.MatchId == matchId && userIdList.Contains(x.UserId)).ToArrayAsync();
            var newEntities = newRuntimeObjs.Select(x => ActiveLiveMatchParticipantEntity.FromRuntime(x, matchId, entities.First(y => y.UserId == x.Id && y.MatchId == matchId).Id)).ToArray();
            context.ActiveLiveMatchParticipant.UpdateRange(newEntities);

            await context.SaveChangesAsync();
            var returnObjs = context.ActiveLiveMatchParticipant.Local.SelectWhere(x => x.User is not null, x => x.ToRuntime()).ToArray();
            return returnObjs.Length > 0 ? (ICollection<LiveMatchParticipant>)returnObjs! : null;

        }
        public async Task<User?> Delete(User runtimeObj, Guid matchId)
        {
            await using var context = _contextFactory.CreateDbContext();
            var entity = await context.ActiveLiveMatchParticipant.FirstOrDefaultAsync(x => x.UserId == runtimeObj.Id && x.MatchId == matchId);
            if (entity is null)
            {
                return null;
            }
            context.ActiveLiveMatchParticipant.Remove(entity);
            await context.SaveChangesAsync();
            return entity.ToRuntime();
        }
        public async Task<User?> Delete(ICollection<Guid> userIdList, Guid matchId)
        {
            await using var context = _contextFactory.CreateDbContext();
            var entity = await context.ActiveLiveMatchParticipant.FirstOrDefaultAsync(x => userIdList.Contains(x.UserId) && x.MatchId == matchId);
            if (entity is null)
            {
                return null;
            }
            context.ActiveLiveMatchParticipant.Remove(entity);
            await context.SaveChangesAsync();
            return entity.ToRuntime();
        }
    }
}