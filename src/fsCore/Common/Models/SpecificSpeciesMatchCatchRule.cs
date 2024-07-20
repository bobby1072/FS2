using Common.Attributes;
using System.Text.Json.Serialization;

namespace Common.Models
{
    public class SpecificSpeciesMatchCatchRule : MatchCatchSingleRule
    {
        [JsonPropertyName("worldFish")]
        public IList<WorldFish> WorldFish { get; set; } = new List<WorldFish>();
        [JsonPropertyName("speciesName")]
        public IList<string> SpeciesName { get; set; } = new List<string>();
        [LockedProperty]
        [JsonPropertyName("ruleType")]
        public override MatchCatchRuleType RuleType { get; } = MatchCatchRuleType.SpecificSpecies;
        public SpecificSpeciesMatchCatchRule(IList<string> speciesName, IList<WorldFish> worldFish, Guid? id = null) : base(id)
        {
            SpeciesName = speciesName;
            WorldFish = worldFish;
        }
        public SpecificSpeciesMatchCatchRule(string speciesName, Guid? id = null) : base(id)
        {
            SpeciesName.Add(speciesName);
        }
        [JsonConstructor]
        public SpecificSpeciesMatchCatchRule() : base()
        {
        }
        public override string BuildRuleDescription()
        {
            if (WorldFish.Any())
            {
                if (WorldFish.Count > 3)
                {
                    return @$"Catch must be one of the following {WorldFish.Count} species: 
                    {string.Join(", ", WorldFish.Select(x => x.EnglishName + (x.ScientificName is not null ? $"({x.ScientificName})" : "")).Take(3))}
                     and {WorldFish.Count - 3} more";
                }
                else
                {
                    return $"Catch must be one of the following species: {string.Join(", ", WorldFish.Select(x => x.EnglishName + (x.ScientificName is not null ? $"({x.ScientificName})" : "")))}";
                }
            }
            else
            {
                if (SpeciesName.Count > 3)
                {
                    return @$"Catch must be one of the following {SpeciesName.Count} species: 
                    {string.Join(", ", SpeciesName.Take(3))}
                     and {SpeciesName.Count - 3} more";
                }
                else
                {
                    return $"Catch must be one of the following species: {string.Join(", ", SpeciesName)}";
                }
            }
        }
        public override IList<(Func<MatchCatch, bool>, string)> BuildRuleValidatorFunctions() => new List<(Func<MatchCatch, bool>, string)>
        {
            (IsSpecificSpecies, "Catch is not of the specific species in the rules"),
        };
        private bool IsSpecificSpecies(MatchCatch matchCatch)
        {
            if (WorldFish.Any())
            {
                if (matchCatch.WorldFish is WorldFish matchWorldFish)
                {
                    return WorldFish.Any(x => x.Taxocode == matchWorldFish.Taxocode);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return SpeciesName.Contains(matchCatch.Species);
            }
        }
    }
}