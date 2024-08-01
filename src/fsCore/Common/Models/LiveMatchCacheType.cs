using System.Text.Json;
using System.Text.Json.Serialization;
using Common.Utils;
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
        public LiveMatchCacheType(Guid groupId, string matchName, LiveMatchRulesCacheType matchRules, LiveMatchStatus matchStatus, LiveMatchWinStrategy winStrategy, IList<LiveMatchCatch> catches, Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
            Catches = catches;
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
            return new LiveMatch(GroupId, MatchName, MatchRules.ToRuntimeType(), MatchStatus, MatchWinStrategy, Catches, Id);
        }
    }
}