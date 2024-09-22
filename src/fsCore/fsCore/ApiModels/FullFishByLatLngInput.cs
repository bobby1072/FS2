using System.Text.Json.Serialization;

namespace fsCore.RequestModels
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