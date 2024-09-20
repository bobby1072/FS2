using Common.Attributes;
using System.Text.Json.Serialization;
namespace Common.Models
{
    public class LiveMatchJsonType
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
        public IList<LiveMatchCatch> Catches { get; set; } = new List<LiveMatchCatch>();
        [JsonPropertyName("participants")]
        public IList<LiveMatchParticipant> Participants { get; set; } = new List<LiveMatchParticipant>();
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
        public LiveMatchJsonType(Guid groupId, string matchName, LiveMatchRulesJsonType matchRules, LiveMatchStatus matchStatus, LiveMatchWinStrategy winStrategy, IList<LiveMatchCatch> catches, IList<LiveMatchParticipant> users, Guid matchLeaderId, DateTime createdAt, DateTime? commencesAt = null, DateTime? endsAt = null, string? description = null, Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
            Catches = catches;
            Description = description;
            GroupId = groupId;
            MatchName = matchName;
            Participants = users;
            MatchRules = matchRules;
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
            return new LiveMatch(GroupId, MatchName, MatchRules.ToRuntimeType(), MatchStatus, MatchWinStrategy, Catches, Participants, MatchLeaderId, CreatedAt, CommencesAt, EndsAt, Description, Id);
        }
    }
}