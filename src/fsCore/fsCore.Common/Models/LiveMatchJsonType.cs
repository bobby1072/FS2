using fsCore.Common.Attributes;
using System.Text.Json.Serialization;
namespace fsCore.Common.Models
{
    public record LiveMatchJsonType
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("matchName")]
        public string MatchName { get; set; }
        [JsonPropertyName("groupId")]
        public Guid GroupId { get; set; }
        [JsonPropertyName("matchRules")]
        public LiveMatchRulesJsonType MatchRules { get; set; }
        [JsonPropertyName("matchStatus")]
        public LiveMatchStatus MatchStatus { get; set; }
        [JsonPropertyName("matchWinStrategy")]
        public LiveMatchWinStrategy MatchWinStrategy { get; set; }
        [JsonPropertyName("catches")]
        public List<LiveMatchCatch> Catches { get; set; }
        [JsonPropertyName("participants")]
        public List<LiveMatchParticipant> Participants { get; set; }
        [JsonPropertyName("matchLeaderId")]
        public Guid MatchLeaderId { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("commencesAt")]
        public DateTime? CommencesAt { get; set; }
        [JsonPropertyName("endsAt")]
        public DateTime? EndsAt { get; set; }
        [LockedProperty]
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("matchWinnerId")]
        public Guid? MatchWinnerId { get; set; } = null;
        public LiveMatchJsonType(Guid groupId, string matchName, LiveMatchRulesJsonType matchRules, LiveMatchStatus matchStatus, LiveMatchWinStrategy winStrategy, List<LiveMatchCatch> catches, List<LiveMatchParticipant> users, Guid matchLeaderId, DateTime createdAt, DateTime? commencesAt = null, DateTime? endsAt = null, string? description = null, Guid? id = null, Guid? matchWinnerId = null)
        {
            Id = id ?? Guid.NewGuid();
            Catches = catches;
            Description = description;
            GroupId = groupId;
            MatchName = matchName;
            Participants = users;
            MatchRules = matchRules;
            MatchWinnerId = matchWinnerId;
            MatchStatus = matchStatus;
            MatchWinStrategy = winStrategy;
            CreatedAt = createdAt;
            CommencesAt = commencesAt;
            EndsAt = endsAt;
            MatchLeaderId = matchLeaderId;
        }
        [JsonConstructor]
        public LiveMatchJsonType() { }
        public LiveMatch ToRuntimeType()
        {
            return new LiveMatch(GroupId, MatchName, MatchRules.ToRuntimeType(), MatchStatus, MatchWinStrategy, Catches, Participants, MatchLeaderId, CreatedAt, CommencesAt, EndsAt, Description, Id, MatchWinnerId);
        }
    }
}