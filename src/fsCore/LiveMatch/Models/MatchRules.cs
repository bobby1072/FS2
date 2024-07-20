using Common.Attributes;
using Common.Models;
using LiveMatch.Models.Rules;
namespace LiveMatch.Models
{
    public class MatchRules : BaseModel
    {
        [LockedProperty]
        public int? Id { get; set; }
        [LockedProperty]
        public Guid MatchId { get; set; }
        public ICollection<MatchCatchRule> Rules { get; set; } = new List<MatchCatchRule>();
    }

}