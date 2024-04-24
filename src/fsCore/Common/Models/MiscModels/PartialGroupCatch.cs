using System.Text.Json.Serialization;

namespace Common.Models.MiscModels
{
    public class PartialGroupCatch
    {
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
        public PartialGroupCatch(string species, double latitude, double longitude, WorldFish? worldFish, DateTime caughtAt, UserWithoutEmail user)
        {
            Species = species;
            Latitude = latitude;
            Longitude = longitude;
            WorldFish = worldFish;
            CaughtAt = caughtAt;
            User = user;
        }
    }
}