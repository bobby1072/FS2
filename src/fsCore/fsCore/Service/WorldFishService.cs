using Common.Dbinterfaces.Repository;
using Common.Models;

namespace fsCore.Service
{
    public class WorldFishService : BaseService<WorldFish, IWorldFishRepository>, IWorldFishService
    {
        public WorldFishService(IWorldFishRepository baseRepo) : base(baseRepo) { }

    }
}