using Common.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;
using Persistence.EntityFramework.Repository.Abstract;

namespace Persistence.EntityFramework.Repository.Concrete
{
    internal class ArchivedLiveMatchRepository : BaseRepository<ArchivedLiveMatch, LiveMatch>
    {
        public ArchivedLiveMatchRepository(IDbContextFactory<FsContext> context) : base(context) { }
        protected override ArchivedLiveMatch RuntimeToEntity(LiveMatch runtimeObj)
        {
            return ArchivedLiveMatch.FromRuntime(runtimeObj);
        }
    }
}