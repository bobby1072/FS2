using System.Text.Json.Serialization;
using Common.Attributes;
namespace Common.Models
{
    public class LiveMatch : BaseModel
    {
        [LockedProperty]
        [JsonPropertyName("id")]
        public Guid Id { get; set; } = Guid.NewGuid();
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
        public IList<LiveMatchCatch> Catches { get; set; } = [];
        [JsonPropertyName("participants")]
        public IList<User> Participants { get; set; } = [];
        [LockedProperty]
        [JsonPropertyName("matchLeaderId")]
        public Guid MatchLeaderId { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [LockedProperty]
        [JsonPropertyName("commencesAt")]
        public DateTime? CommencesAt { get; set; }
        [LockedProperty]
        [JsonPropertyName("endsAt")]
        public DateTime? EndsAt { get; set; }
        [LockedProperty]
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
        public LiveMatch(Guid groupId, string matchName, LiveMatchRules matchRules, LiveMatchStatus matchStatus, LiveMatchWinStrategy winStrategy, IList<LiveMatchCatch> catches, IList<User> users, Guid matchLeaderId, DateTime createdAt, DateTime? commencesAt = null, DateTime? endsAt = null, string? description = null, Guid? id = null)
        {
            if (id is Guid foundId)
            {
                Id = foundId;
            }
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
        public LiveMatchJsonType ToJsonType() => new(GroupId, MatchName, MatchRules.ToJsonType(), MatchStatus, MatchWinStrategy, Catches, Participants, MatchLeaderId, CreatedAt, CommencesAt, EndsAt, Description, Id);
        public void ApplyDefaults(LiveMatchStatus statusOfMatch, User leader)
        {
            MatchLeaderId = (Guid)leader.Id!;
            MatchStatus = statusOfMatch;
            if (MatchStatus == LiveMatchStatus.NotStarted)
            {
                Catches = [];
                CreatedAt = DateTime.UtcNow;
                Participants = [leader];
            }
            else if (MatchStatus == LiveMatchStatus.InProgress)
            {
            }
            else
            {
                EndsAt = DateTime.UtcNow;
            }
        }
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
            && MatchLeaderId == liveMatch.MatchLeaderId
            && CreatedAt == liveMatch.CreatedAt
            && CommencesAt == liveMatch.CommencesAt
            && EndsAt == liveMatch.EndsAt
            && Description == liveMatch.Description;
        }
    }
}