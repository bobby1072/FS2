using fsCore.Common.Attributes;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace fsCore.Common.Models
{
    public class InAreaLiveMatchCatchRule : LiveMatchCatchSingleRule
    {
        [JsonPropertyName("fourPointGeoAreas")]
        public IList<FourPointGeoArea> FourPointGeoAreas { get; set; } = new List<FourPointGeoArea>();
        public InAreaLiveMatchCatchRule(IList<FourPointGeoArea> fourPointGeoAreas)
        {
            FourPointGeoAreas = fourPointGeoAreas;
        }
        [JsonConstructor]
        public InAreaLiveMatchCatchRule() { }
        [AssemblyConstructor]
        public InAreaLiveMatchCatchRule(object? obj)
        {
            if (obj is null)
            {
                throw new InvalidDataException("Object is null");
            }
            else if (obj is JsonElement jsonElement)
            {
                FourPointGeoAreas = jsonElement.GetProperty("fourPointGeoAreas").EnumerateArray().Select(x => JsonSerializer.Deserialize<FourPointGeoArea>(x.GetRawText()) ?? throw new InvalidDataException("Cannot parse FourPointGeoArea")).ToList() ?? throw new InvalidDataException("FourPointGeoAreas is null");
                return;
            }
            else
            {
                dynamic dynamicObj = obj;
                FourPointGeoAreas = dynamicObj.FourPointGeoAreas;
                return;
            }
            throw new InvalidDataException("Object is not a valid type");
        }
        public override string BuildRuleDescription()
        {
            return $"{nameof(InAreaLiveMatchCatchRule)}: {string.Join(", ", JsonSerializer.Serialize(FourPointGeoAreas))}";
        }
        public override IList<(Func<LiveMatchCatch, bool> ValidatorFunctions, string ErrorMessage)> BuildMatchCatchRuleValidatorFunctions()
        {
            return new List<(Func<LiveMatchCatch, bool> ValidatorFunctions, string ErrorMessage)> { (IsWithinAreas, "Catch is not within any of the specified areas") };
        }
        private bool IsWithinAreas(LiveMatchCatch matchCatch)
        {
            for (int i = 0; i < FourPointGeoAreas.Count; i++)
            {
                var area = FourPointGeoAreas[i];
                if (area.IsPointInside(new(matchCatch.Latitude, matchCatch.Longitude)))
                {
                    return true;
                }
            }
            return false;
        }
        public override bool Equals(object? obj)
        {
            return obj is InAreaLiveMatchCatchRule inAreaLiveMatchCatchRule &&
                   FourPointGeoAreas.SequenceEqual(inAreaLiveMatchCatchRule.FourPointGeoAreas);
        }
    }
}