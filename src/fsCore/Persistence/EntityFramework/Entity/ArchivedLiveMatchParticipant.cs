using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Models;

namespace Persistence.EntityFramework.Entity
{
    internal record ArchivedLiveMatchParticipant
    {
        [Key]
        public int Id { get; set; }
        public Guid MatchId { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public UserEntity? User { get; set; }
        public static ArchivedLiveMatchParticipant FromRuntime(LiveMatchParticipant runtime, Guid matchId)
        {
            return new ArchivedLiveMatchParticipant
            {
                Id = (int)runtime.DbId!,
                UserId = runtime.Id,
                MatchId = matchId,
            };
        }
    }
}