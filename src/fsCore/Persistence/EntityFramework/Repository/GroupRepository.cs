using Common.Dbinterfaces.Repository;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;

namespace Persistence.EntityFramework.Repository
{
    internal class GroupRepository : BaseRepository<GroupEntity, Group>, IGroupRepository
    {
        public GroupRepository(IDbContextFactory<FsContext> context) : base(context) { }
        protected override GroupEntity _runtimeToEntity(Group runtimeObj) => GroupEntity.RuntimeToEntity(runtimeObj);

    }
}