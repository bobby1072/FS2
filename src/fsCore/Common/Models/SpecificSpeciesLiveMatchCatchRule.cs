using System.Text.Json.Serialization;

namespace Common.Models
{
    public class SpecificSpeciesLiveMatchCatchRule : LiveMatchSingleRule
    {
        [JsonPropertyName("worldFish")]
        public IList<WorldFish> WorldFish { get; set; } = new List<WorldFish>();
        [JsonPropertyName("speciesName")]
        public IList<string> SpeciesNames { get; set; } = new List<string>();
        public SpecificSpeciesLiveMatchCatchRule(IList<string> speciesName, IList<WorldFish> worldFish, Guid? id = null) : base(id)
        {
            SpeciesNames = speciesName;
            WorldFish = worldFish;
        }
        [JsonConstructor]
        public SpecificSpeciesLiveMatchCatchRule() : base()
        {
        }
        public override string BuildRuleDescription()
        {
            throw new NotImplementedException();
            // if (WorldFish.Any())
            // {
            //     if (WorldFish.Count > 3)
            //     {
            //         return @$"Catch must be one of the following {WorldFish.Count} species: 
            //         {string.Join(", ", WorldFish.Select(x => x.EnglishName + (x.ScientificName is not null ? $"({x.ScientificName})" : "")).Take(3))}
            //          and {WorldFish.Count - 3} more";
            //     }
            //     else
            //     {
            //         return $"Catch must be one of the following species: {string.Join(", ", WorldFish.Select(x => x.EnglishName + (x.ScientificName is not null ? $"({x.ScientificName})" : "")))}";
            //     }
            // }
            // else
            // {
            //     if (SpeciesName.Count > 3)
            //     {
            //         return @$"Catch must be one of the following {SpeciesName.Count} species: 
            //         {string.Join(", ", SpeciesName.Take(3))}
            //          and {SpeciesName.Count - 3} more";
            //     }
            //     else
            //     {
            //         return $"Catch must be one of the following species: {string.Join(", ", SpeciesName)}";
            //     }
            // }
        }
        public override IList<LiveMatchCatchSingleRuleValidatorFunction> BuildRuleValidatorFunctions() => new List<LiveMatchCatchSingleRuleValidatorFunction>
        {
            new (IsSpecificSpecies, $"Catch is {(SpeciesNames.Count > 1 ? "not included in specified species list":"not the species specified in the")} in the rules"),
        };
        private bool IsSpecificSpecies(LiveMatchCatch matchCatch)
        {
            if (WorldFish.Any())
            {
                if (matchCatch.WorldFishTaxocode is not null)
                {
                    return WorldFish.Any(x => x.Taxocode == matchCatch.WorldFishTaxocode);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return SpeciesNames.Contains(matchCatch.Species);
            }
        }
    }
}