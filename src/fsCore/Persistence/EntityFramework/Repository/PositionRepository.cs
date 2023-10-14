using Common.Dbinterfaces.Repository;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;

namespace Persistence.EntityFramework.Repository
{
    internal class PositionRepository : BaseRepository<PositionEntity, Position>, IPositionRepository
    {
        public PositionRepository(IDbContextFactory<FsContext> context) : base(context) { }
        protected override PositionEntity _runtimeToEntity(Position runtimeObj)
        {
            return PositionEntity.RuntimeToEntity(runtimeObj);
        }
    }
}