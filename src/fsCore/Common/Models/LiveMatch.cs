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
        [LockedProperty]
        [JsonPropertyName("matchWinStrategy")]
        public LiveMatchWinStrategy MatchWinStrategy { get; set; }
        [JsonPropertyName("catches")]
        public IList<LiveMatchCatch> Catches { get; set; } = new List<LiveMatchCatch>();
        [JsonPropertyName("participants")]
        public IList<User> Participants { get; set; } = new List<User>();
        [LockedProperty]
        [JsonPropertyName("matchLeaderId")]
        public Guid MatchLeaderId { get; set; }
        public LiveMatch(Guid groupId, string matchName, LiveMatchRules matchRules, LiveMatchStatus matchStatus, LiveMatchWinStrategy winStrategy, IList<LiveMatchCatch> catches, IList<User> users, Guid MatchLeaderId, Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
            Catches = catches;
            GroupId = groupId;
            MatchName = matchName;
            Participants = users;
            MatchRules = matchRules;
            MatchStatus = matchStatus;
            MatchWinStrategy = winStrategy;
        }
        public LiveMatch(LiveMatch liveMatch)
        {
            Id = liveMatch.Id;
            Catches = liveMatch.Catches;
            GroupId = liveMatch.GroupId;
            MatchName = liveMatch.MatchName;
            Participants = liveMatch.Participants;
            MatchRules = liveMatch.MatchRules;
            MatchStatus = liveMatch.MatchStatus;
            MatchWinStrategy = liveMatch.MatchWinStrategy;
            MatchLeaderId = liveMatch.MatchLeaderId;
        }
        [JsonConstructor]
        public LiveMatch() { }
        public LiveMatchCacheType ToCacheType() => new(GroupId, MatchName, MatchRules.ToCacheType(), MatchStatus, MatchWinStrategy, Catches, Participants, Id);
        public override bool Equals(object? obj)
        {
            if (obj is not LiveMatch liveMatch)
            {
                return false;
            }
            return Id == liveMatch.Id
            && MatchName == liveMatch.MatchName
            && GroupId == liveMatch.GroupId
            && MatchRules.Equals(liveMatch.MatchRules)
            && MatchStatus == liveMatch.MatchStatus
            && MatchWinStrategy == liveMatch.MatchWinStrategy
            && Catches.SequenceEqual(liveMatch.Catches)
            && Participants.SequenceEqual(liveMatch.Participants)
            && MatchLeaderId == liveMatch.MatchLeaderId;
        }
    }
}