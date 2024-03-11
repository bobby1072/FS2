using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Models;

namespace Persistence.EntityFramework.Entity
{
    [Table("group_catch", Schema = DbConstants.MainSchema)]
    internal class GroupCatchEntity : BaseEntity<GroupCatch>
    {
        [Key]
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        [ForeignKey(nameof(GroupId))]
        public GroupEntity? Group { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public UserEntity? User { get; set; }
        public string Species { get; set; }
        public double Weight { get; set; }
        public double Length { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime CaughtAt { get; set; }
        public byte[]? CatchPhoto { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public override GroupCatch ToRuntime()
        {
            return new GroupCatch(UserId, GroupId, Species, Weight, CaughtAt, Length, Latitude, Longitude, Description, Id, CreatedAt, CatchPhoto, Group?.ToRuntime(), User?.ToRuntime());
        }
        public static GroupCatchEntity RuntimeToEntity(GroupCatch groupCatch)
        {
            return new GroupCatchEntity
            {
                Id = groupCatch.Id ?? Guid.NewGuid(),
                Species = groupCatch.Species,
                Weight = groupCatch.Weight,
                Length = groupCatch.Length,
                Description = groupCatch.Description,
                CreatedAt = DateTime.SpecifyKind(groupCatch.CreatedAt, DateTimeKind.Utc),
                CaughtAt = DateTime.SpecifyKind(groupCatch.CaughtAt, DateTimeKind.Utc),
                CatchPhoto = groupCatch.CatchPhoto,
                GroupId = groupCatch.GroupId,
                Latitude = groupCatch.Latitude,
                Longitude = groupCatch.Longitude
            };
        }
    }
}