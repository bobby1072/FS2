using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Models;

namespace Persistence.EntityFramework.Entity
{
    [Table("group_position", Schema = DbConstants.MainSchema)]
    internal class GroupPositionEntity : BaseEntity<GroupPosition>
    {
        [Key]
        [Column(TypeName = "INTEGER")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Column(TypeName = "UUID")]
        public Guid GroupId { get; set; }
        [ForeignKey(nameof(GroupId))]
        public GroupEntity? Group { get; set; }
        [Required]
        [Column(TypeName = "TEXT")]
        public string Name { get; set; }
        [Required]
        [Column(TypeName = "BOOLEAN")]
        public bool CanManageGroup { get; set; }
        [Required]
        [Column(TypeName = "BOOLEAN")]
        public bool CanReadCatches { get; set; }
        [Required]
        [Column(TypeName = "BOOLEAN")]
        public bool CanManageCatches { get; set; }
        [Required]
        [Column(TypeName = "BOOLEAN")]
        public bool CanReadMembers { get; set; }
        [Required]
        [Column(TypeName = "BOOLEAN")]
        public bool CanManageMembers { get; set; }
        public override GroupPosition ToRuntime()
        {
            return new GroupPosition(GroupId, Name, Id, CanManageGroup, CanReadCatches, CanManageCatches, CanReadMembers, CanManageMembers, Group?.ToRuntime());
        }
        public static GroupPositionEntity RuntimeToEntity(GroupPosition position)
        {
            var ent = new GroupPositionEntity
            {
                GroupId = position.GroupId,
                Name = position.Name
            };
            if (position.Id.HasValue && position.Id.Value > 0)
            {
                ent.Id = position.Id.Value;
            }
            return ent;
        }
    }
}