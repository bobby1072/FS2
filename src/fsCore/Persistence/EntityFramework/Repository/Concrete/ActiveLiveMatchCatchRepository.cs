using Common.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;
using Persistence.EntityFramework.Repository.Abstract;

namespace Persistence.EntityFramework.Repository.Concrete
{
    internal class ActiveLiveMatchCatchRepository : BaseRepository<ActiveLiveMatchCatchEntity, LiveMatchCatch>, IActiveLiveMatchCatchRepository
    {
        public ActiveLiveMatchCatchRepository(IDbContextFactory<FsContext> context) : base(context) { }
        protected override ActiveLiveMatchCatchEntity RuntimeToEntity(LiveMatchCatch runtimeObj)
        {
            return ActiveLiveMatchCatchEntity.FromRuntime(runtimeObj);
        }
    }
}