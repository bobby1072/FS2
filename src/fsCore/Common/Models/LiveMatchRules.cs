using System.Text.Json.Serialization;
using Common.Attributes;
using Common.Models.Validators;
using FluentValidation;
namespace Common.Models
{
    public class LiveMatchRules : BaseModel
    {
        [LockedProperty]
        [JsonPropertyName("id")]
        public int? Id { get; set; }
        [JsonPropertyName("rules")]
        public IList<LiveMatchCatchSingleRule> Rules { get; set; } = new List<LiveMatchCatchSingleRule>();
        public LiveMatchRules(IList<LiveMatchCatchSingleRule> rules, int? id = null)
        {
            Id = id;
            Rules = rules;
        }
        [JsonConstructor]
        public LiveMatchRules() { }
        public IValidator<LiveMatchCatch> BuildMatchRulesValidatorForSingleCatch()
        {
            var allSingleRuleValidatorFuncs = Rules.SelectMany(x => x.BuildRuleValidatorFunctions());
            return new DynamicLiveMatchCatchRuleValidatorForSingle(allSingleRuleValidatorFuncs.Select(x => (x.ValidatorFunctionForSingle, x.ErrorMessage)).ToArray());
        }
        public IValidator<IEnumerable<LiveMatchCatch>> BuildMatchRulesValidatorForEnumerableCatches()
        {
            var allSingleRuleValidatorFuncs = Rules.SelectMany(x => x.BuildRuleValidatorFunctions());
            return new DynamicLiveMatchCatchRuleValidatorForEnumerable(allSingleRuleValidatorFuncs.Select(x => (x.ValidatorFunctionForList, x.ErrorMessage)).ToArray());
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