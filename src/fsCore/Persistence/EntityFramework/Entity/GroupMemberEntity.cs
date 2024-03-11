using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Models;

namespace Persistence.EntityFramework.Entity
{
    [Table("group_member", Schema = DbConstants.MainSchema)]
    internal class GroupMemberEntity : BaseEntity<GroupMember>
    {
        [Key]
        public int Id { get; set; }
        public Guid GroupId { get; set; }
        [ForeignKey(nameof(GroupId))]
        public GroupEntity? Group { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public UserEntity? User { get; set; }
        public int PositionId { get; set; }
        [ForeignKey(nameof(PositionId))]
        public GroupPositionEntity? Position { get; set; }
        public override GroupMember ToRuntime()
        {
            return new GroupMember(GroupId, PositionId, UserId, Id, User?.ToRuntime(), Group?.ToRuntime(), Position?.ToRuntime());
        }
        public static GroupMemberEntity RuntimeToEntity(GroupMember groupMember)
        {
            var ent = new GroupMemberEntity
            {

                Id = groupMember.Id ?? 0,
                GroupId = groupMember.GroupId,
                UserId = groupMember.UserId
            };
            if (groupMember.Id.HasValue && groupMember.Id > 0)
            {
                ent.PositionId = groupMember.PositionId;
            }
            return ent;
        }
    }
}