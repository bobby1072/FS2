using Common.Dbinterfaces.Repository;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;

namespace Persistence.EntityFramework.Repository
{
    internal class GroupCatchRepository : BaseRepository<GroupCatchEntity, GroupCatch>, IGroupCatchRepository
    {
        public GroupCatchRepository(IDbContextFactory<FsContext> context) : base(context) { }
        protected override GroupCatchEntity _runtimeToEntity(GroupCatch runtimeObj)
        {
            return GroupCatchEntity.RuntimeToEntity(runtimeObj);
        }
    }
}