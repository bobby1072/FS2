using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Models;

namespace Persistence.EntityFramework.Entity
{
    [Table("group_member", Schema = DbConstants.MainSchema)]
    internal class GroupMemberEntity : BaseEntity<GroupMember>
    {
        [Key]
        [Required]
        [Column(TypeName = "INTEGER")]
        public int Id { get; set; }
        [Required]
        [Column(TypeName = "UUID")]
        public Guid GroupId { get; set; }
        [ForeignKey(nameof(GroupId))]
        public GroupEntity? Group { get; set; }
        [Required]
        [Column(TypeName = "TEXT")]
        public string UserEmail { get; set; }
        [ForeignKey(nameof(UserEmail))]
        public UserEntity? User { get; set; }
        [Required]
        [Column(TypeName = "INTEGER")]
        public int PositionId { get; set; }
        [ForeignKey(nameof(PositionId))]
        public PositionEntity? Position { get; set; }
        public override GroupMember ToRuntime()
        {
            return new GroupMember(Id, GroupId, UserEmail, PositionId, User?.ToRuntime(), Group?.ToRuntime(), Position?.ToRuntime());
        }
        public static GroupMemberEntity RuntimeToEntity(GroupMember groupMember)
        {
            return new GroupMemberEntity
            {
                Id = groupMember.Id,
                GroupId = groupMember.GroupId,
                UserEmail = groupMember.UserEmail
            };
        }
    }
}