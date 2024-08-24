using System.Text.Json.Serialization;
using Common.Models;

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
        public (LatLng, Guid) BreakDown()
        {
            return (new LatLng(Latitude, Longitude), GroupId);
        }
    }
}