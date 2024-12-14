using fsCore.Common.Models;
using fsCore.Common.Utils;
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
        public async Task<ICollection<LiveMatchParticipant>?> Create(ICollection<(LiveMatchParticipant LiveMatchParticipant, Guid MatchId)> participants)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var entities = participants.Select(x => ActiveLiveMatchParticipantEntity.FromRuntime(x.LiveMatchParticipant, x.MatchId)).ToArray();
            await context.ActiveLiveMatchParticipant.AddRangeAsync(entities);
            await context.SaveChangesAsync();
            var returnObjs = context.ActiveLiveMatchParticipant.Local.SelectWhere(x => x.User is not null, x => x.ToRuntime()).ToArray();
            return returnObjs.Length > 0 ? (ICollection<LiveMatchParticipant>)returnObjs! : null;
        }
        public async Task<ICollection<LiveMatchParticipant>?> Update(ICollection<(LiveMatchParticipant LiveMatchParticipant, Guid MatchId)> participants)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var entities = participants.Select(x => ActiveLiveMatchParticipantEntity.FromRuntime(x.LiveMatchParticipant, x.MatchId)).ToArray();
            context.ActiveLiveMatchParticipant.UpdateRange(entities);
            await context.SaveChangesAsync();
            var returnObjs = context.ActiveLiveMatchParticipant.Local.SelectWhere(x => x.User is not null, x => x.ToRuntime()).ToArray();
            return returnObjs.Length > 0 ? (ICollection<LiveMatchParticipant>)returnObjs! : null;
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
            var entities = await context.ActiveLiveMatchParticipant.Include(x => x.User).Where(x => x.MatchId == matchId).ToArrayAsync();
            var returnObjs = entities.SelectWhere(x => x.User is not null, x => x.ToRuntime()).ToArray();
            return returnObjs.Length > 0 ? (ICollection<LiveMatchParticipant>)returnObjs! : null;
        }
        public async Task<ICollection<LiveMatchParticipant>?> GetForMatch(ICollection<Guid> matchId)
        {
            await using var context = _contextFactory.CreateDbContext();
            var entities = await context.ActiveLiveMatchParticipant.Include(x => x.User).Where(x => matchId.Contains(x.MatchId)).ToArrayAsync();
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
            var newEntities = newRuntimeObjs.DistinctBy(x => x.Id).Select(x => ActiveLiveMatchParticipantEntity.FromRuntime(x, matchId)).ToArray();
            context.ActiveLiveMatchParticipant.UpdateRange(newEntities);
            await context.SaveChangesAsync();
            var returnObjs = context.ActiveLiveMatchParticipant.Local.SelectWhere(x => x.User is not null, x => x.ToRuntime()).ToArray();
            return returnObjs.Length > 0 ? (ICollection<LiveMatchParticipant>)returnObjs! : null;

        }
        public async Task<ICollection<LiveMatchParticipant>?> Update(ICollection<Guid> matchIds, LiveMatchParticipant runtimeObj)
        {
            await using var context = _contextFactory.CreateDbContext();
            var newEntities = matchIds.Distinct().Select(x => ActiveLiveMatchParticipantEntity.FromRuntime(runtimeObj, x)).ToArray();
            context.ActiveLiveMatchParticipant.UpdateRange(newEntities);
            await context.SaveChangesAsync();
            var returnObjs = context.ActiveLiveMatchParticipant.Local.SelectWhere(x => x.User is not null, x => x.ToRuntime()).ToArray();
            return returnObjs.Length > 0 ? (ICollection<LiveMatchParticipant>)returnObjs! : null;
        }
        public async Task Delete(User runtimeObj, Guid matchId)
        {
            await using var context = _contextFactory.CreateDbContext();
            var entity = await context.ActiveLiveMatchParticipant.FirstOrDefaultAsync(x => x.UserId == runtimeObj.Id && x.MatchId == matchId);
            if (entity is not null)
            {
                context.ActiveLiveMatchParticipant.Remove(entity);
                await context.SaveChangesAsync();
            }
        }
        public async Task Delete(ICollection<Guid> userIdList, Guid matchId)
        {
            await using var context = _contextFactory.CreateDbContext();
            var entity = await context.ActiveLiveMatchParticipant.FirstOrDefaultAsync(x => userIdList.Contains(x.UserId) && x.MatchId == matchId);
            if (entity is not null)
            {
                context.ActiveLiveMatchParticipant.Remove(entity);
                await context.SaveChangesAsync();
            }
        }
        public async Task Delete(ICollection<LiveMatchParticipant> participants)
        {
            await using var context = _contextFactory.CreateDbContext();
            var entities = participants.Select(x => ActiveLiveMatchParticipantEntity.FromRuntime(x, Guid.Empty)).ToArray();
            context.ActiveLiveMatchParticipant.RemoveRange(entities);
            await context.SaveChangesAsync();
        }
    }
}