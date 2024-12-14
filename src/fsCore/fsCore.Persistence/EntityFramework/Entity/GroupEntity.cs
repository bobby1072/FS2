using fsCore.Common.Models;
using fsCore.Persistence;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fsCore.Persistence.EntityFramework.Entity
{
    [Table("group", Schema = DbConstants.PublicSchema)]
    internal record GroupEntity : BaseEntity<Group>
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid LeaderId { get; set; }
        [ForeignKey(nameof(LeaderId))]
        public virtual UserEntity? Leader { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Public { get; set; }
        public bool Listed { get; set; }
        public byte[]? Emblem { get; set; }
        public bool CatchesPublic { get; set; }
        public virtual ICollection<GroupPositionEntity>? Positions { get; set; }
        public virtual ICollection<GroupCatchEntity>? Catches { get; set; }
        public override Group ToRuntime()
        {
            return new Group(Name, Emblem, Description, Id, CreatedAt, Public, Listed, CatchesPublic, LeaderId, Leader?.ToRuntime(), Positions?.Select(p => p.ToRuntime()).ToArray());
        }
        public static GroupEntity RuntimeToEntity(Group group)
        {
            return new GroupEntity
            {
                Id = group.Id ?? Guid.NewGuid(),
                LeaderId = group.LeaderId,
                Name = group.Name,
                CreatedAt = DateTime.SpecifyKind(group.CreatedAt, DateTimeKind.Utc),
                Public = group.Public,
                CatchesPublic = group.CatchesPublic,
                Listed = group.Listed,
                Emblem = group.Emblem,
                Description = group.Description
            };
        }
    }
}