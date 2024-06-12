using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Models;

namespace Persistence.EntityFramework.Entity
{
    [Table("group_position", Schema = DbConstants.PublicSchema)]
    internal class GroupPositionEntity : BaseEntity<GroupPosition>
    {
        [Key]
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        [ForeignKey(nameof(GroupId))]
        public GroupEntity? Group { get; set; }
        public string Name { get; set; }
        public bool CanManageGroup { get; set; }
        public bool CanReadCatches { get; set; }
        public bool CanManageCatches { get; set; }
        public bool CanReadMembers { get; set; }
        public bool CanManageMembers { get; set; }
        public override GroupPosition ToRuntime()
        {
            return new GroupPosition(GroupId, Name, Id, CanManageGroup, CanReadCatches, CanManageCatches, CanReadMembers, CanManageMembers);
        }
        public static GroupPositionEntity RuntimeToEntity(GroupPosition position)
        {
            var ent = new GroupPositionEntity
            {
                Id = position.Id ?? Guid.NewGuid(),
                GroupId = position.GroupId,
                Name = position.Name,
                CanManageCatches = position.CanManageCatches,
                CanManageGroup = position.CanManageGroup,
                CanManageMembers = position.CanManageMembers,
                CanReadCatches = position.CanReadCatches,
                CanReadMembers = position.CanReadMembers
            };
            return ent;
        }
    }
}