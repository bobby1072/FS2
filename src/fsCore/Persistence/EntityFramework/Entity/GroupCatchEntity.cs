using fsCore.Common.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.EntityFramework.Entity
{
    [Table("group_catch", Schema = DbConstants.PublicSchema)]
    internal record GroupCatchEntity : BaseEntity<GroupCatch>
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
        public decimal Weight { get; set; }
        public decimal Length { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime CaughtAt { get; set; }
        public byte[]? CatchPhoto { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? WorldFishTaxocode { get; set; }
        [ForeignKey(nameof(WorldFishTaxocode))]
        public WorldFishEntity? WorldFish { get; set; }
        public override GroupCatch ToRuntime()
        {
            return new GroupCatch(UserId, GroupId, Species, Weight, CaughtAt, Length, Latitude, Longitude, Description, Id, CreatedAt, CatchPhoto, Group?.ToRuntime(), User?.ToRuntime(), WorldFishTaxocode, WorldFish?.ToRuntime());
        }
        public static GroupCatchEntity RuntimeToEntity(GroupCatch groupCatch)
        {
            return new GroupCatchEntity
            {
                Id = groupCatch.Id ?? Guid.NewGuid(),
                Species = groupCatch.Species,
                UserId = groupCatch.UserId,
                WorldFishTaxocode = groupCatch.WorldFishTaxocode,
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