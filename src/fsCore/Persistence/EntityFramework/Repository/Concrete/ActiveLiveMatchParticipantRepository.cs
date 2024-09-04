using Common.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;
using Persistence.EntityFramework.Repository.Abstract;

namespace Persistence.EntityFramework.Repository.Concrete
{
    internal class ActiveLiveMatchParticipantRepository
    {
        private readonly IDbContextFactory<FsContext> _contextFactory;
        public ActiveLiveMatchParticipantRepository(IDbContextFactory<FsContext> context)
        {
            _contextFactory = context;
        }
        private ActiveLiveMatchParticipantEntity RuntimeToEntity(User runtimeObj, Guid matchId)
        {
            return ActiveLiveMatchParticipantEntity.FromRuntime(runtimeObj, matchId);
        }
    }
}