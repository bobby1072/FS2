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
        [Column(TypeName = "TEXT")]
        public string LeaderEmail { get; set; }
        [ForeignKey(nameof(LeaderEmail))]
        public UserEntity? Leader { get; set; }
        [Required]
        [Column(TypeName = "TIMESTAMP")]
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
            return new Group(Name, LeaderEmail, Emblem, Description, Id, CreatedAt, Public, Listed, Leader?.ToRuntime(), Members?.Select(m => m.ToRuntime()).ToList(), Positions?.Select(p => p.ToRuntime()).ToList(), Catches?.Select(c => c.ToRuntime()).ToList());
        }
        public static GroupEntity RuntimeToEntity(Group group)
        {
            return new GroupEntity
            {
                Id = group.Id ?? Guid.NewGuid(),
                Name = group.Name,
                LeaderEmail = group.LeaderEmail,
                CreatedAt = group.CreatedAt,
                Public = group.Public,
                Listed = group.Listed,
                Emblem = group.Emblem,
                Description = group.Description
            };
        }
    }
}