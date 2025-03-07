using fsCore.Common.Attributes;
using System.Text.Json.Serialization;
namespace fsCore.Common.Models
{
    public class LiveMatch : BaseModel
    {
        public override int GetHashCode() => base.GetHashCode();
        private Guid? _id;
        [LockedProperty]
        [JsonPropertyName("id")]
        public Guid Id
        {
            get
            {
                _id ??= Guid.NewGuid();
                return (Guid)_id!;
            }
            set
            {
                _id = value;
            }
        }
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
        private IList<LiveMatchCatch>? _catches;
        [JsonPropertyName("catches")]
        public IList<LiveMatchCatch> Catches
        {
            get
            {
                _catches ??= new List<LiveMatchCatch>();
                return _catches;
            }
            set
            {
                _catches = value;
            }
        }
        private IList<LiveMatchParticipant>? _participants;
        [JsonPropertyName("participants")]
        public IList<LiveMatchParticipant> Participants
        {
            get
            {
                _participants ??= new List<LiveMatchParticipant>();
                return _participants;
            }
            set
            {
                _participants = value;
            }
        }
        [LockedProperty]
        [JsonPropertyName("matchWinnerId")]
        public Guid? MatchWinnerId { get; set; } = null;
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
        [JsonIgnore]
        public TimeSpan? TimeUntilStart { get => CommencesAt - DateTime.UtcNow; }
        [JsonIgnore]
        public TimeSpan? TimeUntilEnd { get => EndsAt - DateTime.UtcNow; }
        public LiveMatch(Guid groupId, string matchName, LiveMatchRules matchRules, LiveMatchStatus matchStatus, LiveMatchWinStrategy winStrategy, IList<LiveMatchCatch> catches, IList<LiveMatchParticipant> users, Guid matchLeaderId, DateTime createdAt, DateTime? commencesAt = null, DateTime? endsAt = null, string? description = null, Guid? id = null, Guid? matchWinner = null)
        {
            if (id is Guid foundId)
            {
                Id = foundId;
            }
            Catches = catches;
            Description = description;
            MatchWinnerId = matchWinner;
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
            MatchWinnerId = liveMatch.MatchWinnerId;
            CreatedAt = liveMatch.CreatedAt;
            CommencesAt = liveMatch.CommencesAt;
            EndsAt = liveMatch.EndsAt;
            Description = liveMatch.Description;
        }
        [JsonConstructor]
        public LiveMatch() { }
        public LiveMatchJsonType ToJsonType() => new(GroupId, MatchName, MatchRules.ToJsonType(), MatchStatus, MatchWinStrategy, Catches.ToList(), Participants.ToList(), MatchLeaderId, CreatedAt, CommencesAt, EndsAt, Description, Id, MatchWinnerId);
        public void ApplyDefaults(LiveMatchStatus statusOfMatch, User leader)
        {
            MatchLeaderId = (Guid)leader.Id!;
            MatchStatus = statusOfMatch;
            if (MatchStatus == LiveMatchStatus.NotStarted)
            {
                Catches = [];
                Participants = [];
                CreatedAt = DateTime.UtcNow;
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