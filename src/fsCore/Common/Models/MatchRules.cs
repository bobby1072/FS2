using System.Text.Json.Serialization;
using Common.Attributes;
using Common.Models.Validators;
using FluentValidation;
namespace Common.Models
{
    public class MatchRules : BaseModel
    {
        [LockedProperty]
        [JsonPropertyName("id")]
        public int? Id { get; set; }
        [LockedProperty]
        [JsonPropertyName("matchId")]
        public Guid MatchId { get; set; }
        [JsonPropertyName("rules")]
        public IList<MatchCatchSingleRule> Rules { get; set; } = new List<MatchCatchSingleRule>();
        public IValidator<MatchCatch> BuildMatchRulesValidator()
        {
            var allSingleRuleValidatorFuncs = Rules.SelectMany(x => x.BuildRuleValidatorFunctions()).ToArray();
            return new RuleValidator(allSingleRuleValidatorFuncs);
        }
    }

}