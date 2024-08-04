using System.Text.Json.Serialization;
using Common.Attributes;
using Common.Models.Validators;
using FluentValidation;
namespace Common.Models
{
    public class LiveMatchRules : BaseModel
    {
        [JsonPropertyName("rules")]
        public IList<LiveMatchSingleRule> Rules { get; set; } = new List<LiveMatchSingleRule>();
        public LiveMatchRules(IList<LiveMatchSingleRule> rules)
        {
            Rules = rules;
        }
        [JsonConstructor]
        public LiveMatchRules() { }
        public IValidator<LiveMatchCatch> BuildMatchRulesValidator()
        {
            var allSingleRuleValidatorFuncs = Rules.SelectMany(x => x.BuildRuleValidatorFunctions()).ToArray();
            return new DynamicLiveMatchCatchRuleValidator(allSingleRuleValidatorFuncs);
        }
        public LiveMatchRulesCacheType ToCacheType() => new(Rules.Select(x => (object)x).ToList());
    }

}