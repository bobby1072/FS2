using System.Text.Json.Serialization;

namespace fsCore.Api.ApiModels
{
    public record SaveParticipantLiveMatchHubInput
    {
        [JsonPropertyName("userId")]
        public Guid UserId { get; init; }
        [JsonPropertyName("matchId")]
        public Guid MatchId { get; init; }
    }
}