using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Common.Models;

namespace Persistence.EntityFramework.Entity
{
    [Table("active_live_match", Schema = DbConstants.PublicSchema)]
    internal record ActiveLiveMatchEntity : BaseEntity<LiveMatch>
    {
        [Key]
        public Guid Id { get; set; }
        public string MatchName { get; set; }
        public Guid GroupId { get; set; }
        public string SerialisedMatchRules { get; set; }
        public int MatchStatus { get; set; }
        public int MatchWinStrategy { get; set; }
        public virtual IList<ActiveLiveMatchCatchEntity>? Catches { get; set; }
        public string SerialisedParticipants { get; set; }
        public Guid MatchLeaderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CommencesAt { get; set; }
        public DateTime? EndsAt { get; set; }
        public override LiveMatch ToRuntime()
        {
            var participants = JsonSerializer.Deserialize<IList<User>>(SerialisedParticipants) ?? throw new InvalidOperationException("Couldn't deserialise live match participants");
            var deserialisedRules = JsonSerializer.Deserialize<IList<object>>(SerialisedMatchRules) ?? throw new InvalidOperationException("Couldn't deserialise live match rules");
            var rules = new LiveMatchRulesJsonType(deserialisedRules);
            return new LiveMatch(GroupId, MatchName, rules.ToRuntimeType(), (LiveMatchStatus)MatchStatus, (LiveMatchWinStrategy)MatchWinStrategy, Catches?.Select(x => x.ToRuntime()).ToList() ?? new List<LiveMatchCatch>(), participants, MatchLeaderId, CreatedAt, CommencesAt, EndsAt, null, Id);
        }
        public static ActiveLiveMatchEntity FromRuntime(LiveMatch runtime)
        {
            var entity = new ActiveLiveMatchEntity
            {
                Id = runtime.Id,
                MatchName = runtime.MatchName,
                GroupId = runtime.GroupId,
                SerialisedMatchRules = JsonSerializer.Serialize((object)runtime.MatchRules.Rules),
                MatchStatus = (int)runtime.MatchStatus,
                MatchWinStrategy = (int)runtime.MatchWinStrategy,
                SerialisedParticipants = JsonSerializer.Serialize((object)runtime.Participants),
                MatchLeaderId = runtime.MatchLeaderId,
                CreatedAt = runtime.CreatedAt,
                CommencesAt = runtime.CommencesAt,
                EndsAt = runtime.EndsAt
            };
            return entity;
        }
    }
}