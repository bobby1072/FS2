using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Models;

namespace Persistence.EntityFramework.Entity
{
    [Table("group", Schema = DbConstants.MainSchema)]
    internal class GroupEntity : BaseEntity<Group>
    {
        [Key]
        [Required]
        [Column(TypeName = "UUID")]
        public Guid Id { get; set; }
        [Required]
        [Column(TypeName = "TEXT")]
        public string Name { get; set; }
        [Column(TypeName = "TEXT")]
        public string? Description { get; set; }
        [Required]
        [Column(TypeName = "UUID")]
        public Guid LeaderId { get; set; }
        [ForeignKey(nameof(LeaderId))]
        public virtual UserEntity? Leader { get; set; }
        [Required]
        [Column(TypeName = "TIMESTAMP with time zone")]
        public DateTime CreatedAt { get; set; }
        [Required]
        [Column(TypeName = "BOOLEAN")]
        public bool Public { get; set; }
        [Required]
        [Column(TypeName = "BOOLEAN")]
        public bool Listed { get; set; }
        [Column(TypeName = "BYTEA")]
        public byte[]? Emblem { get; set; }
        public virtual ICollection<GroupMemberEntity>? Members { get; set; }
        public virtual ICollection<GroupPositionEntity>? Positions { get; set; }
        public virtual ICollection<GroupCatchEntity>? Catches { get; set; }
        public override Group ToRuntime()
        {
            return new Group(Name, Emblem, Description, Id, CreatedAt, Public, Listed, LeaderId, Leader?.ToRuntime(), Members?.Select(m => m.ToRuntime()).ToArray(), Positions?.Select(p => p.ToRuntime()).ToArray(), Catches?.Select(c => c.ToRuntime()).ToArray());
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
                Listed = group.Listed,
                Emblem = group.Emblem,
                Description = group.Description
            };
        }
    }
}