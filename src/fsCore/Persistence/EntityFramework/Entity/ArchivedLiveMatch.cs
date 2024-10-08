
using System.ComponentModel.DataAnnotations.Schema;
using Persistence;

namespace Persistence.EntityFramework.Entity
{
    [Table("archived_live_match", Schema = DbConstants.PublicSchema)]
    internal record ArchivedLiveMatch
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public string MatchName { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime EndedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}