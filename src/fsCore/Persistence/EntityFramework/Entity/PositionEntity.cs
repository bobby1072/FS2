using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Models;

namespace Persistence.EntityFramework.Entity
{
    [Table("group_position", Schema = DbConstants.MainSchema)]
    internal class PositionEntity : BaseEntity<Position>
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
        public override Position ToRuntime()
        {
            return new Position(Id, GroupId, Name);
        }
        public static PositionEntity RuntimeToEntity(Position position)
        {
            return new PositionEntity
            {
                Id = position.Id,
                GroupId = position.GroupId,
                Name = position.Name
            };
        }
    }
}