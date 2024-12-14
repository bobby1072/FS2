using System.Text.Json.Serialization;

namespace fsCore.Common.Models
{
    public class PartialGroupCatch
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("species")]
        public string Species { get; set; }
        [JsonPropertyName("latitude")]
        public decimal Latitude { get; set; }
        [JsonPropertyName("longitude")]
        public decimal Longitude { get; set; }
        [JsonPropertyName("worldFish")]
        public WorldFish? WorldFish { get; set; }
        [JsonPropertyName("caughtAt")]
        public DateTime CaughtAt { get; set; }
        [JsonPropertyName("user")]
        public UserWithoutEmail User { get; set; }
        [JsonPropertyName("weight")]
        public decimal Weight { get; set; }
        [JsonPropertyName("length")]
        public decimal Length { get; set; }
        [JsonPropertyName("groupId")]
        public Guid GroupId { get; set; }
        public PartialGroupCatch(string species, decimal latitude, decimal longitude, WorldFish? worldFish, DateTime caughtAt, UserWithoutEmail user, decimal weight, Guid id, decimal length, Guid groupId)
        {
            Species = species;
            GroupId = groupId;
            Latitude = latitude;
            Id = id;
            Longitude = longitude;
            WorldFish = worldFish;
            CaughtAt = caughtAt;
            User = user;
            Weight = weight;
            Length = length;
        }
    }
}