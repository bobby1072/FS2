using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using Common.Utils;
namespace Common.Models
{
    public class LiveMatchRulesCacheType
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("matchId")]
        public Guid MatchId { get; set; }
        [JsonPropertyName("rules")]
        public IList<object> Rules { get; set; } = new List<object>();
        public LiveMatchRulesCacheType(Guid matchId, IList<object> rules, Guid? id = null)
        {
            MatchId = matchId;
            Rules = rules;
            Id = id ?? Guid.NewGuid();
        }
        [JsonConstructor]
        public LiveMatchRulesCacheType() { }
        public LiveMatchRules ToRuntimeType()
        {
            var parsedRules = Rules.Select(x => AssemblyUtils.ParseToChildOf<LiveMatchSingleRule>(JsonSerializer.Serialize(x))).ToList();
            return new LiveMatchRules(MatchId, parsedRules, Id);
        }
    }

}