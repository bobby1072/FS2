using System.Text.Json.Serialization;
using Common.Attributes;
namespace Common.Models
{
    public class LiveMatch : BaseModel
    {
        [LockedProperty]
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("matchName")]
        public string MatchName { get; set; }
        [LockedProperty]
        [JsonPropertyName("groupId")]
        public Guid GroupId { get; set; }
        [JsonPropertyName("matchRules")]
        public LiveMatchRules MatchRules { get; set; }
        [JsonPropertyName("matchStatus")]
        public LiveMatchStatus MatchStatus { get; set; }

    }
}