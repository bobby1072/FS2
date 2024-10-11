using Common.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.EntityFramework.Entity
{
    [Table("active_live_match_participant", Schema = DbConstants.PublicSchema)]
    internal record ActiveLiveMatchParticipantEntity : BaseEntity<User>
    {
        [Key]
        public int Id { get; set; }
        public Guid MatchId { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public UserEntity? User { get; set; }
        public bool UserOnline { get; set; }
        public override LiveMatchParticipant? ToRuntime()
        {
            return LiveMatchParticipant.FromUser(User?.ToRuntime(), UserOnline, Id);
        }
        public static ActiveLiveMatchParticipantEntity FromRuntime(LiveMatchParticipant runtime, Guid matchId)
        {
            var entity = new ActiveLiveMatchParticipantEntity
            {
                UserId = runtime.Id!,
                MatchId = matchId,
                UserOnline = runtime.Online
            };
            if (runtime.DbId is int foundId)
            {
                entity.Id = foundId;
            }
            return entity;
        }
    }
}