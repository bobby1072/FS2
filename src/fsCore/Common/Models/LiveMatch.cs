using System.Text.Json.Serialization;
using Common.Attributes;
namespace Common.Models
{
    public class LiveMatch : BaseModel
    {
        [LockedPropertyAttribute]
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("matchName")]
        public string MatchName { get; set; }
        [LockedPropertyAttribute]
        [JsonPropertyName("groupId")]
        public Guid GroupId { get; set; }
        [JsonPropertyName("matchRules")]
        public LiveMatchRules MatchRules { get; set; }
        [JsonPropertyName("matchStatus")]
        public LiveMatchStatus MatchStatus { get; set; }
        [JsonPropertyName("matchWinStrategy")]
        public LiveMatchWinStrategy MatchWinStrategy { get; set; }
        public LiveMatch(Guid groupId, string matchName, LiveMatchRules matchRules, LiveMatchStatus matchStatus, LiveMatchWinStrategy winStrategy, Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
            GroupId = groupId;
            MatchName = matchName;
            MatchRules = matchRules;
            MatchStatus = matchStatus;
            MatchWinStrategy = winStrategy;
        }
        public LiveMatchCacheType ToCacheType() => new(GroupId, MatchName, MatchRules.ToCacheType(), MatchStatus, MatchWinStrategy, Id);
    }
}