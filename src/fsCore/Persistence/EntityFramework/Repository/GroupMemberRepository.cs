using Common.Dbinterfaces.Repository;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;

namespace Persistence.EntityFramework.Repository
{
    internal class GroupMemberRepository : BaseRepository<GroupMemberEntity, GroupMember>, IGroupMemberRepository
    {
        public GroupMemberRepository(IDbContextFactory<FsContext> context) : base(context) { }
        protected override GroupMemberEntity _runtimeToEntity(GroupMember runtimeObj)
        {
            return GroupMemberEntity.RuntimeToEntity(runtimeObj);
        }
    }
}