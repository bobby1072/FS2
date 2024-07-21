using System.Text.Json.Serialization;
using Common.Attributes;
namespace Common.Models
{
    public abstract class LiveMatchWinStrategy : BaseModel
    {
        [JsonIgnore]
        protected Type _thisType => GetType();
        [LockedProperty]
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [LockedProperty]
        [JsonPropertyName("strategyType")]
        public string StrategyType => _thisType.Name;
        protected LiveMatchWinStrategy(Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
        }
        public abstract string BuildStrategyDescription();
        public abstract Guid GetWinnerUserId(List<LiveMatchCatch> catches);
    }
}