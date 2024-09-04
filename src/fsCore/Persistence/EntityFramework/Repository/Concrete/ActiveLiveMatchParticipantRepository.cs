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
        private static ActiveLiveMatchParticipantEntity RuntimeToEntity(User runtimeObj, Guid matchId)
        {
            return ActiveLiveMatchParticipantEntity.FromRuntime(runtimeObj, matchId);
        }
        public async Task<ICollection<User>?> Create(ICollection<User> runtimeObjs, Guid matchId)
        {
            await using var context = _contextFactory.CreateDbContext();
            var entities = runtimeObjs.Select(x => RuntimeToEntity(x, matchId)).ToList();
            await context.ActiveLiveMatchParticipant.AddRangeAsync(entities);
            await context.SaveChangesAsync();
            var returnObjs = context.ActiveLiveMatchParticipant.Local.SelectWhere(x => x.User is not null, x => x.ToRuntime()).ToList();
            return returnObjs.Count > 0 ? returnObjs : null;
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
    }
}