using fsCore.Common.Attributes;
using System.Text.Json.Serialization;
namespace fsCore.Common.Models
{
    public class LiveMatchCatch : Catch
    {
        [LockedProperty]
        [JsonPropertyName("matchId")]
        public Guid MatchId { get; set; }
        [JsonPropertyName("countsInMatch")]
        public bool CountsInMatch { get; set; }
        [JsonConstructor]
        public LiveMatchCatch() { }
        public LiveMatchCatch(Guid userId, Guid matchId, string species, decimal weight, DateTime caughtAt, decimal length, decimal latitude, decimal longitude, string? description, bool? countsInMatch, Guid? id, DateTime? createdAt, string? worldFishTaxocode)
        {
            Id = id;
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
        public override bool Equals(object? obj)
        {
            if (obj is not LiveMatchCatch liveMatchCatch)
            {
                return false;
            }
            return Id == liveMatchCatch.Id
            && MatchId == liveMatchCatch.MatchId
            && CountsInMatch == liveMatchCatch.CountsInMatch
            && base.Equals(liveMatchCatch);
        }
    }
}