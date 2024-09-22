using System.Text.Json.Serialization;

namespace fsCore.ApiModels
{
    public record FullFishByLatLngInput
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
        [JsonPropertyName("groupId")]
        public Guid GroupId { get; set; }
    }
}