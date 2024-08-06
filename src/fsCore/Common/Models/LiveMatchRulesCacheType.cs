using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using Common.Utils;
namespace Common.Models
{
    public class LiveMatchRulesCacheType
    {
        [JsonPropertyName("rules")]
        public IList<object> Rules { get; set; } = new List<object>();
        public LiveMatchRulesCacheType(IList<object> rules)
        {
            Rules = rules;
        }
        [JsonConstructor]
        public LiveMatchRulesCacheType() { }
        public LiveMatchRules ToRuntimeType()
        {
            var parsedRules = Rules.Select(x => CommonAssemblyUtils.ParseToChildOf<LiveMatchSingleRule>(x)).ToList();
            return new LiveMatchRules(parsedRules);
        }
    }

}