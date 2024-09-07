using Common.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.EntityFramework.Entity
{
    [Table("active_live_match_catch", Schema = DbConstants.PublicSchema)]
    internal record ActiveLiveMatchCatchEntity : BaseEntity<LiveMatchCatch>
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public UserEntity? User { get; set; }
        public Guid MatchId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Species { get; set; }
        public bool CountsInMatch { get; set; }
        public double Weight { get; set; }
        public double Length { get; set; }
        public string? WorldFishTaxocode { get; set; }
        [ForeignKey(nameof(WorldFishTaxocode))]
        public WorldFishEntity? WorldFish { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime CaughtAt { get; set; }
        public override LiveMatchCatch ToRuntime()
        {
            return new LiveMatchCatch(UserId, MatchId, Species, Weight, CaughtAt, Length, Latitude, Longitude, Description, CountsInMatch, Id, CreatedAt, WorldFishTaxocode);
        }
        public static ActiveLiveMatchCatchEntity FromRuntime(LiveMatchCatch runtime)
        {
            var entity = new ActiveLiveMatchCatchEntity
            {
                UserId = runtime.UserId,
                MatchId = runtime.MatchId,
                Latitude = runtime.Latitude,
                Longitude = runtime.Longitude,
                Species = runtime.Species,
                CountsInMatch = runtime.CountsInMatch,
                Weight = runtime.Weight,
                Length = runtime.Length,
                WorldFishTaxocode = runtime.WorldFishTaxocode,
                Description = runtime.Description,
                CreatedAt = runtime.CreatedAt,
                CaughtAt = runtime.CaughtAt
            };
            if (runtime.Id is Guid foundId)
            {
                entity.Id = foundId;
            }
            return entity;
        }
    }
}