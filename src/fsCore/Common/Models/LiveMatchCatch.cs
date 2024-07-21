using System.Text.Json.Serialization;
using Common.Attributes;
namespace Common.Models
{
    public class LiveMatchCatch : BaseModel
    {
        [LockedProperty]
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [LockedProperty]
        [JsonPropertyName("matchId")]
        public Guid MatchId { get; set; }
        [LockedProperty]
        [JsonPropertyName("userId")]
        public Guid UserId { get; set; }
        [LockedProperty]
        [JsonPropertyName("species")]
        public string Species { get; set; }
        [JsonPropertyName("weight")]
        public double Weight { get; set; }
        [JsonPropertyName("length")]
        public double Length { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [LockedProperty]
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("caughtAt")]
        public DateTime CaughtAt { get; set; }
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
        [JsonPropertyName("worldFishTaxocode")]
        public string? WorldFishTaxocode { get; set; }
        [JsonPropertyName("countsInMatch")]
        public bool CountsInMatch { get; set; }
        [JsonConstructor]
        public LiveMatchCatch() { }
        public LiveMatchCatch(Guid userId, Guid matchId, string species, double weight, DateTime caughtAt, double length, double latitude, double longitude, string? description, bool? countsInMatch, Guid? id, DateTime? createdAt, string? worldFishTaxocode)
        {
            Id = id ?? Guid.NewGuid();
            MatchId = matchId;
            Latitude = latitude;
            Longitude = longitude;
            UserId = userId;
            Species = species;
            CountsInMatch = countsInMatch ?? false;
            Weight = weight;
            Length = length;
            WorldFishTaxocode = worldFishTaxocode;
            Description = description;
            CreatedAt = createdAt ?? DateTime.UtcNow;
            CaughtAt = caughtAt;
        }
        public LiveMatchCatch ApplyDefaults(Guid? userId = null)
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            CountsInMatch = false;
            if (userId is Guid notNullUserId)
            {
                UserId = notNullUserId;
            }
            return this;
        }
    }
}