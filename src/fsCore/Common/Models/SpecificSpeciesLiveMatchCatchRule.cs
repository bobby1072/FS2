using System.Text.Json;
using System.Text.Json.Serialization;
using Common.Attributes;
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
        [AssemblyConstructor]
        public SpecificSpeciesLiveMatchCatchRule(object? obj)
        {
            if (obj is null)
            {
                throw new InvalidDataException("Object is null");
            }
            else if (obj is JsonElement jsonElement)
            {
                SpeciesNames = jsonElement.GetProperty("speciesName").EnumerateArray().Select(x => x.GetString() ?? throw new InvalidDataException("Cannot parse species name string")).ToList() ?? throw new InvalidDataException("SpeciesName is null");
                WorldFish = jsonElement.GetProperty("worldFish").EnumerateArray().Select(x => JsonSerializer.Deserialize<WorldFish>(x.GetRawText()) ?? throw new InvalidDataException("Cannot parse worldFish")).ToList() ?? throw new InvalidDataException("WorldFish is null");
                Id = Guid.Parse(jsonElement.GetProperty("id").GetString() ?? throw new InvalidDataException("Id is null"));
            }
            else if (obj is SpecificSpeciesLiveMatchCatchRule specificSpeciesLiveMatchCatchRule)
            {
                SpeciesNames = specificSpeciesLiveMatchCatchRule.SpeciesNames;
                WorldFish = specificSpeciesLiveMatchCatchRule.WorldFish;
                Id = specificSpeciesLiveMatchCatchRule.Id;
            }
            else
            {
                throw new InvalidDataException("Object is not a valid type");
            }
        }
        [JsonConstructor]
        public SpecificSpeciesLiveMatchCatchRule() : base()
        {
        }
        public override string BuildRuleDescription()
        {
            return $"{nameof(SpecificSpeciesLiveMatchCatchRule)}: {string.Join(", ", SpeciesNames)}";
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