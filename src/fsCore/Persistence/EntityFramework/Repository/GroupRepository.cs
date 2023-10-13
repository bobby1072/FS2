using Common.Dbinterfaces.Repository;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;

namespace Persistence.EntityFramework.Repository
{
    internal class GroupRepository : BaseRepository<GroupEntity, GroupModel>, IGroupRepository
    {
        public GroupRepository(IDbContextFactory<FsContext> context) : base(context) { }
        protected override GroupEntity _runtimeToEntity(GroupModel runtimeObj)
        {
            return GroupEntity.RuntimeToEntity(runtimeObj);
        }
    }
}