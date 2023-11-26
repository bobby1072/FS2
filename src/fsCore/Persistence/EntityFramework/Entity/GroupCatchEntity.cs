using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Models;

namespace Persistence.EntityFramework.Entity
{
    [Table("group_catch", Schema = DbConstants.MainSchema)]
    internal class GroupCatchEntity : BaseEntity<GroupCatch>
    {
        [Key]
        [Required]
        [Column(TypeName = "UUID")]
        public Guid Id { get; set; }
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
        [Column(TypeName = "TEXT")]
        public string Species { get; set; }
        [Required]
        [Column(TypeName = "DECIMAL")]
        public double Weight { get; set; }
        [Required]
        [Column(TypeName = "DECIMAL")]
        public double Length { get; set; }
        [Column(TypeName = "TEXT")]
        public string? Description { get; set; }
        [Required]
        [Column(TypeName = "TIMESTAMP")]
        public DateTime CreatedAt { get; set; }
        [Required]
        [Column(TypeName = "TIMESTAMP")]
        public DateTime CaughtAt { get; set; }
        [Column(TypeName = "BYTEA")]
        public byte[]? CatchPhoto { get; set; }
        public override GroupCatch ToRuntime()
        {
            return new GroupCatch(UserEmail, GroupId, Species, Weight, CaughtAt, Length, Description, Id, CreatedAt, CatchPhoto, Group?.ToRuntime(), User?.ToRuntime());
        }
        public static GroupCatchEntity RuntimeToEntity(GroupCatch groupCatch)
        {
            return new GroupCatchEntity
            {
                Id = groupCatch.Id ?? Guid.NewGuid(),
                UserEmail = groupCatch.UserEmail,
                Species = groupCatch.Species,
                Weight = groupCatch.Weight,
                Length = groupCatch.Length,
                Description = groupCatch.Description,
                CreatedAt = groupCatch.CreatedAt,
                CaughtAt = groupCatch.CaughtAt,
                CatchPhoto = groupCatch.CatchPhoto,
                GroupId = groupCatch.GroupId
            };
        }
    }
}