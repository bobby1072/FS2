using System.Text.Json.Serialization;

namespace Common.Models.MiscModels
{
    public class PartialGroupCatch
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("species")]
        public string Species { get; set; }
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
        [JsonPropertyName("worldFish")]
        public WorldFish? WorldFish { get; set; }
        [JsonPropertyName("caughtAt")]
        public DateTime CaughtAt { get; set; }
        [JsonPropertyName("user")]
        public UserWithoutEmail User { get; set; }
        [JsonPropertyName("weight")]
        public double Weight { get; set; }
        [JsonPropertyName("length")]
        public double Length { get; set; }
        [JsonPropertyName("groupId")]
        public Guid GroupId { get; set; }
        public PartialGroupCatch(string species, double latitude, double longitude, WorldFish? worldFish, DateTime caughtAt, UserWithoutEmail user, double weight, Guid id, double length, Guid groupId)
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