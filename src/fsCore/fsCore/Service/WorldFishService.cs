using Common.Dbinterfaces.Repository;
using Common.Models;

namespace fsCore.Service
{
    public class WorldFishService : BaseService<WorldFish, IWorldFishRepository>
    {
        public WorldFishService(IWorldFishRepository baseRepo) : base(baseRepo) { }

    }
}