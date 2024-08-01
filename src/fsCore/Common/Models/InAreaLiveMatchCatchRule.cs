using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.Models
{
    public class InAreaLiveMatchCatchRule : LiveMatchSingleRule
    {
        [JsonPropertyName("fourPointGeoAreas")]
        public IList<FourPointGeoArea> FourPointGeoAreas { get; set; } = new List<FourPointGeoArea>();
        public InAreaLiveMatchCatchRule(IList<FourPointGeoArea> fourPointGeoAreas, Guid? id = null) : base(id)
        {
            FourPointGeoAreas = fourPointGeoAreas;
        }
        [JsonConstructor]
        public InAreaLiveMatchCatchRule() { }
        public override string BuildRuleDescription()
        {
            return $"{nameof(InAreaLiveMatchCatchRule)}: {string.Join(", ", JsonSerializer.Serialize(FourPointGeoAreas))}";
        }
        public override IList<LiveMatchCatchSingleRuleValidatorFunction> BuildRuleValidatorFunctions()
        {
            return new List<LiveMatchCatchSingleRuleValidatorFunction> { new(IsWithinAreas, "Catch is not within any of the specified areas") };
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
    }
}