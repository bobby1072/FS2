using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Models;

namespace Persistence.EntityFramework.Entity
{
    [Table("archived_live_match_catch", Schema = DbConstants.PublicSchema)]
    internal record ArchivedLiveMatchCatch
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public UserEntity? User { get; set; }
        public string Species { get; set; }
        public decimal Weight { get; set; }
        public decimal Length { get; set; }
        public string? Description { get; set; }
        public Guid MatchId { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime CaughtAt { get; set; }
        public string? WorldFishTaxocode { get; set; }
        public static ArchivedLiveMatchCatch FromRuntime(LiveMatchCatch runtime)
        {
            return new ArchivedLiveMatchCatch
            {
                UserId = runtime.UserId,
                Species = runtime.Species,
                Weight = runtime.Weight,
                Length = runtime.Length,
                Description = runtime.Description,
                MatchId = runtime.MatchId,
                Longitude = runtime.Longitude,
                Latitude = runtime.Latitude,
                CreatedAt = runtime.CreatedAt,
                CaughtAt = runtime.CaughtAt,
                WorldFishTaxocode = runtime.WorldFishTaxocode,
                Id = (Guid)runtime.Id!
            };
        }
    }
}