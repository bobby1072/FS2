using fsCore.Common.Attributes;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace fsCore.Common.Models
{
    public class SpecificSpeciesLiveMatchCatchRule : LiveMatchCatchSingleRule
    {
        [JsonPropertyName("worldFish")]
        public IList<WorldFish> WorldFish { get; set; } = new List<WorldFish>();
        [JsonPropertyName("speciesName")]
        public IList<string> SpeciesNames { get; set; } = new List<string>();
        public SpecificSpeciesLiveMatchCatchRule(IList<string> speciesName, IList<WorldFish> worldFish)
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
                return;
            }
            else
            {
                dynamic dynamicObj = obj;
                SpeciesNames = dynamicObj.SpeciesNames;
                WorldFish = dynamicObj.WorldFish;
                return;
            }
            throw new InvalidDataException("Object is not a valid type");
        }
        [JsonConstructor]
        public SpecificSpeciesLiveMatchCatchRule() : base()
        {
        }
        public override string BuildRuleDescription()
        {
            return $"{nameof(SpecificSpeciesLiveMatchCatchRule)}: {string.Join(", ", SpeciesNames)}";
        }
        public override IList<(Func<LiveMatchCatch, bool> ValidatorFunctions, string ErrorMessage)> BuildMatchCatchRuleValidatorFunctions() => new List<(Func<LiveMatchCatch, bool> ValidatorFunctions, string ErrorMessage)>
        {
            (IsSpecificSpecies, $"Catch is {(SpeciesNames.Count > 1 ? "not included in specified species list":"not the species specified in the")} in the rules"),
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
        public override bool Equals(object? obj)
        {
            if (obj is not SpecificSpeciesLiveMatchCatchRule specificSpeciesLiveMatchCatchRule)
            {
                return false;
            }
            return SpeciesNames.SequenceEqual(specificSpeciesLiveMatchCatchRule.SpeciesNames)
            && WorldFish.SequenceEqual(specificSpeciesLiveMatchCatchRule.WorldFish);
        }
    }
}