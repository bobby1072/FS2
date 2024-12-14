using fsCore.Common.Models;
using fsCore.Persistence;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fsCore.Persistence.EntityFramework.Entity
{
    [Table("group_member", Schema = DbConstants.PublicSchema)]
    internal record GroupMemberEntity : BaseEntity<GroupMember>
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
            if (groupMember.Id is int foundInt)
            {

                return new GroupMemberEntity
                {
                    Id = foundInt,
                    GroupId = groupMember.GroupId,
                    PositionId = groupMember.PositionId,
                    UserId = groupMember.UserId
                };
            }
            else
            {
                return new GroupMemberEntity
                {
                    GroupId = groupMember.GroupId,
                    PositionId = groupMember.PositionId,
                    UserId = groupMember.UserId
                };
            }
        }
    }
}