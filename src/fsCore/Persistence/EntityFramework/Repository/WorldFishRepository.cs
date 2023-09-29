using Common.Dbinterfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace Persistence.EntityFramework.Repository
{
    internal class WorldFishRepository : IWorldFishRepository
    {
        private readonly IDbContextFactory<FsContext> _dbContextFactory;
        public WorldFishRepository(IDbContextFactory<FsContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        }
    }
}