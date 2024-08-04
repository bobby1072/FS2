using System.Text.Json.Serialization;
namespace Common.Models
{
    public class LiveMatchCacheType
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("matchName")]
        public string MatchName { get; set; }
        [JsonPropertyName("groupId")]
        public Guid GroupId { get; set; }
        [JsonPropertyName("matchRules")]
        public LiveMatchRulesCacheType MatchRules { get; set; }
        [JsonPropertyName("matchStatus")]
        public LiveMatchStatus MatchStatus { get; set; }
        [JsonPropertyName("matchWinStrategy")]
        public LiveMatchWinStrategy MatchWinStrategy { get; set; }
        [JsonPropertyName("catches")]
        public IList<LiveMatchCatch> Catches { get; set; } = new List<LiveMatchCatch>();
        [JsonPropertyName("participants")]
        public IList<User> Participants { get; set; } = new List<User>();
        [JsonPropertyName("matchLeaderId")]
        public Guid MatchLeaderId { get; set; }
        public LiveMatchCacheType(Guid groupId, string matchName, LiveMatchRulesCacheType matchRules, LiveMatchStatus matchStatus, LiveMatchWinStrategy winStrategy, IList<LiveMatchCatch> catches, IList<User> participants, Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
            Catches = catches;
            Participants = participants;
            GroupId = groupId;
            MatchName = matchName;
            MatchRules = matchRules;
            MatchStatus = matchStatus;
            MatchWinStrategy = winStrategy;
        }
        [JsonConstructor]
        public LiveMatchCacheType() { }
        public LiveMatch ToRuntimeType()
        {
            return new LiveMatch(GroupId, MatchName, MatchRules.ToRuntimeType(), MatchStatus, MatchWinStrategy, Catches, Participants, Id);
        }
    }
}