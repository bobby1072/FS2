
using Common.Attributes;
using System.Text.Json.Serialization;

namespace Common.Models
{
    public abstract class MatchCatchSingleRule : BaseModel
    {
        [LockedProperty]
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [LockedProperty]
        [JsonPropertyName("ruleType")]
        public abstract MatchCatchRuleType RuleType { get; }
        protected MatchCatchSingleRule(Guid? id = null)
        {
            Id = id is Guid foundId ? foundId : Guid.NewGuid();
        }
        public abstract string BuildRuleDescription();
        public abstract IList<(Func<MatchCatch, bool>, string)> BuildRuleValidatorFunctions();
    }
}