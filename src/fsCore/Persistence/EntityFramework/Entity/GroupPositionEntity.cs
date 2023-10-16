using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Models;

namespace Persistence.EntityFramework.Entity
{
    [Table("group_position", Schema = DbConstants.MainSchema)]
    internal class GroupPositionEntity : BaseEntity<GroupPosition>
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
        public string Name { get; set; }
        public override GroupPosition ToRuntime()
        {
            return new GroupPosition(Id, GroupId, Name);
        }
        public static GroupPositionEntity RuntimeToEntity(GroupPosition position)
        {
            return new GroupPositionEntity
            {
                Id = position.Id,
                GroupId = position.GroupId,
                Name = position.Name
            };
        }
    }
}