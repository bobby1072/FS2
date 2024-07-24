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
        public IList<LiveMatchSingleRule> Rules { get; set; } = new List<LiveMatchSingleRule>();
        public LiveMatchRules(Guid matchId, IList<LiveMatchSingleRule> rules, Guid? id = null)
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
            return new DynamicLiveMatchCatchRuleValidator(allSingleRuleValidatorFuncs);
        }
        public LiveMatchRulesCacheType ToCacheType() => new(MatchId, Rules.Select(x => (object)x).ToList(), Id);
    }

}