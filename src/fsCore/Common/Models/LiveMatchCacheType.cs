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
        public object MatchWinStrategy { get; set; }
        public LiveMatchCacheType(Guid groupId, string matchName, LiveMatchRulesCacheType matchRules, LiveMatchStatus matchStatus, object winStrategy, Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
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
            var parsedWinStrategy = BaseModel.ParseToChildOf<LiveMatchWinStrategy>(JsonSerializer.Serialize(MatchWinStrategy));
            return new LiveMatch(GroupId, MatchName, MatchRules.ToRuntimeType(), MatchStatus, parsedWinStrategy, Id);
        }
    }
}