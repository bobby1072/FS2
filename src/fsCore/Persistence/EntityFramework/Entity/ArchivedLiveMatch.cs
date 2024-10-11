using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Models;

namespace Persistence.EntityFramework.Entity
{
    [Table("archived_live_match", Schema = DbConstants.PublicSchema)]
    internal record ArchivedLiveMatch
    {
        [Key]
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public string MatchName { get; set; }
        public int MatchWinStrategy { get; set; }
        public Guid MatchWinnerId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime EndedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public static ArchivedLiveMatch FromRuntime(LiveMatch runtime)
        {
            return new ArchivedLiveMatch
            {
                GroupId = runtime.GroupId,
                MatchName = runtime.MatchName,
                StartedAt = (DateTime)runtime.CommencesAt!,
                EndedAt = (DateTime)runtime.EndsAt!,
                CreatedAt = runtime.CreatedAt,
                Id = runtime.Id
            };
        }
    }
}