using fsCore.Common.Attributes;
using fsCore.Common.Models.Validators;
using FluentValidation;
using System.Text.Json.Serialization;
namespace fsCore.Common.Models
{
    public class LiveMatchRules : BaseModel
    {
        [LockedProperty]
        [JsonPropertyName("id")]
        public int? Id { get; set; }
        private IList<LiveMatchCatchSingleRule>? _rules;
        [JsonPropertyName("rules")]
        public IList<LiveMatchCatchSingleRule> Rules
        {
            get
            {
                _rules ??= new List<LiveMatchCatchSingleRule>();
                return _rules;
            }
            set
            {
                _rules = value;
            }
        }
        public LiveMatchRules(IList<LiveMatchCatchSingleRule> rules, int? id = null)
        {
            Id = id;
            Rules = rules;
        }
        [JsonConstructor]
        public LiveMatchRules() { }
        public IValidator<LiveMatchCatch> BuildMatchCatchValidator()
        {
            var allSingleRuleValidatorFuncs = Rules.SelectMany(x => x.BuildMatchCatchRuleValidatorFunctions()).ToArray();
            return new DynamicLiveMatchCatchRuleValidatorForSingle(allSingleRuleValidatorFuncs);
        }
        public LiveMatchRulesJsonType ToJsonType() => new(Rules.Select(x => (object)x).ToList());
        public override bool Equals(object? obj)
        {
            if (obj is not LiveMatchRules liveMatchRules)
            {
                return false;
            }
            return Rules.SequenceEqual(liveMatchRules.Rules);
        }
    }

}