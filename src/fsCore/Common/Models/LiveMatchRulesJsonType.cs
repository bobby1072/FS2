using System.Data;
using System.Text.Json.Serialization;
namespace Common.Models
{
    public class LiveMatchRulesJsonType
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }
        [JsonPropertyName("rules")]
        public IList<object> Rules { get; set; } = new List<object>();
        public LiveMatchRulesJsonType(IList<object> rules, int? id = null)
        {
            Id = id;
            Rules = rules;
        }
        [JsonConstructor]
        public LiveMatchRulesJsonType() { }
        public LiveMatchRules ToRuntimeType()
        {
            var parsedRules = Rules.Select(LiveMatchCatchSingleRule.ToSingleRule).ToList();
            return new LiveMatchRules(parsedRules, Id);
        }
    }

}