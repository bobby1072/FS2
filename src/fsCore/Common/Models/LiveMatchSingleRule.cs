
using Common.Attributes;
using System.Text.Json.Serialization;

namespace Common.Models
{
    public abstract class LiveMatchSingleRule : BaseModel
    {
        [JsonIgnore]
        protected Type _thisType => GetType();
        [LockedProperty]
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        protected LiveMatchSingleRule(Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
        }
        public abstract string BuildRuleDescription();
        public abstract IList<LiveMatchCatchSingleRuleValidatorFunction> BuildRuleValidatorFunctions();
    }
}