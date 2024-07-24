using System.Text.Json.Serialization;
using Common.Attributes;
namespace Common.Models
{
    public class LiveMatch : BaseModel
    {
        [LockedProperty]
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [LockedProperty]
        [JsonPropertyName("matchName")]
        public string MatchName { get; set; }
        [LockedProperty]
        [JsonPropertyName("groupId")]
        public Guid GroupId { get; set; }
        [JsonPropertyName("matchRules")]
        public LiveMatchRules MatchRules { get; set; }
        [JsonPropertyName("matchStatus")]
        public LiveMatchStatus MatchStatus { get; set; }
        [JsonPropertyName("matchWinStrategy")]
        public LiveMatchWinStrategy MatchWinStrategy { get; set; }
        [JsonPropertyName("catches")]
        public IList<LiveMatchCatch> Catches { get; set; } = new List<LiveMatchCatch>();
        public LiveMatch(Guid groupId, string matchName, LiveMatchRules matchRules, LiveMatchStatus matchStatus, LiveMatchWinStrategy winStrategy, IList<LiveMatchCatch> catches, Guid? id = null)
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
        public LiveMatch() { }
        public LiveMatchCacheType ToCacheType() => new(GroupId, MatchName, MatchRules.ToCacheType(), MatchStatus, MatchWinStrategy, Catches, Id);
    }
}