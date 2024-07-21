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
        public Guid Id { get; set; }
        [LockedProperty]
        [JsonPropertyName("matchId")]
        public Guid MatchId { get; set; }
        [JsonPropertyName("rules")]
        public IList<LiveMatchCatchSingleRule> Rules { get; set; } = new List<LiveMatchCatchSingleRule>();
        public LiveMatchRules(Guid matchId, IList<LiveMatchCatchSingleRule> rules, Guid? id = null)
        {
            MatchId = matchId;
            Rules = rules;
            Id = id ?? Guid.NewGuid();
        }
        [JsonConstructor]
        public LiveMatchRules() { }
        public IValidator<LiveMatchCatch> BuildMatchRulesValidator()
        {
            var allSingleRuleValidatorFuncs = Rules.SelectMany(x => x.BuildRuleValidatorFunctions()).ToArray();
            return new RuleValidator(allSingleRuleValidatorFuncs);
        }
    }

}