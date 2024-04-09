using System.Text.Json.Serialization;
using Common.Models.MiscModels;

namespace fsCore.Controllers.ControllerModels
{
    public class FullFishByLatLngInput
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