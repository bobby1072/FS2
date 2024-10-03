using System.Net;
using System.Text.Json.Serialization;
using Common.Misc;
using Common.Models;

namespace fsCore.ApiModels
{
    public record SaveMatchLiveHubInput
    {
        [JsonPropertyName("id")]
        public Guid? Id { get; init; }
        [JsonPropertyName("matchName")]
        public string MatchName { get; init; }
        [JsonPropertyName("groupId")]
        public Guid GroupId { get; init; }
        [JsonPropertyName("matchRules")]
        public LiveMatchRulesJsonType MatchRules { get; init; }
        [JsonPropertyName("matchStatus")]
        public int MatchStatus { get; init; }
        [JsonPropertyName("matchWinStrategy")]
        public int MatchWinStrategy { get; init; }
        [JsonPropertyName("matchLeaderId")]
        public Guid MatchLeaderId { get; init; }
        [JsonPropertyName("description")]
        public string Description { get; init; }
        [JsonPropertyName("commencesAt")]
        public DateTime? CommencesAt { get; init; }
        [JsonPropertyName("endsAt")]
        public DateTime? EndsAt { get; init; }
        public LiveMatch ToLiveMatch(Guid matchLeaderId)
        {
            if (!Enum.TryParse<LiveMatchStatus>(MatchStatus.ToString(), out var matchStatus) || !Enum.TryParse<LiveMatchWinStrategy>(MatchWinStrategy.ToString(), out var matchWinStrategy))
            {
                throw new ApiException(ErrorConstants.BadRequest, HttpStatusCode.BadRequest);
            }
            return new LiveMatch(GroupId, MatchName, MatchRules.ToRuntimeType(), matchStatus, matchWinStrategy, new List<LiveMatchCatch>(), new List<LiveMatchParticipant>(), matchLeaderId, DateTime.UtcNow, CommencesAt, EndsAt, Description, Id);
        }
    }
}