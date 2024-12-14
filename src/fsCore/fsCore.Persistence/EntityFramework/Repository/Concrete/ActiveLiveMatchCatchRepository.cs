using fsCore.Common.Models;
using fsCore.Persistence.EntityFramework;
using fsCore.Persistence.EntityFramework.Entity;
using fsCore.Persistence.EntityFramework.Repository.Abstract;
using Microsoft.EntityFrameworkCore;

namespace fsCore.Persistence.EntityFramework.Repository.Concrete
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