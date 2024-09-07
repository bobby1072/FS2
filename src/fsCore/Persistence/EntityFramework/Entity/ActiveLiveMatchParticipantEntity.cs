using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Models;

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
        public override User? ToRuntime()
        {
            return User?.ToRuntime();
        }
        public static ActiveLiveMatchParticipantEntity FromRuntime(User runtime, Guid matchId)
        {
            var entity = new ActiveLiveMatchParticipantEntity
            {
                UserId = (Guid)runtime.Id,
                MatchId = matchId
            };
            return entity;
        }
    }
}