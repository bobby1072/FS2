using System.Text.Json.Serialization;
using Common.Models.MiscModels;

namespace fsCore.Controllers.ControllerModels
{
    public class SquareRangeCatchesInput
    {
        [JsonPropertyName("bottomLeftLatLong")]
        public LatLng BottomLeftLatLong { get; set; }
        [JsonPropertyName("topRightLatLong")]
        public LatLng TopRightLatLong { get; set; }
        [JsonPropertyName("groupId")]
        public Guid GroupId { get; set; }
    }
}