using System.Text.Json.Serialization;

namespace fsCore.ApiModels
{
    public record SaveMatchCatchLiveMatchHubInput
    {
        [JsonPropertyName("id")]
        public Guid? Id { get; init; }
        [JsonPropertyName("matchId")]
        public Guid MatchId { get; init; }
        [JsonPropertyName("userId")]
        public string Species { get; init; }
        [JsonPropertyName("weight")]
        public double Weight { get; init; }
        [JsonPropertyName("length")]
        public double Length { get; init; }
        [JsonPropertyName("description")]
        public string? Description { get; init; }
        [JsonPropertyName("caughtAt")]
        public DateTime CaughtAt { get; init; }
        [JsonPropertyName("createdAt")]
        public DateTime? CreatedAt { get; init; }
        [JsonPropertyName("latitude")]
        public double Latitude { get; init; }
        [JsonPropertyName("longitude")]
        public double Longitude { get; init; }
        [JsonPropertyName("worldFishTaxocode")]
        public string? WorldFishTaxocode { get; init; }
        [JsonPropertyName("countsInMatch")]
        public bool CountsInMatch { get; init; }
    }
}