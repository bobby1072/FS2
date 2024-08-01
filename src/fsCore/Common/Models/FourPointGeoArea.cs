using System.Text.Json.Serialization;
using Common.Models.MiscModels;

namespace Common.Models
{
    public class FourPointGeoArea : BaseModel
    {
        [JsonPropertyName("topLeft")]
        public LatLng TopLeft { get; set; }
        [JsonPropertyName("bottomLeft")]
        public LatLng BottomLeft { get; set; }
        [JsonPropertyName("topRight")]
        public LatLng TopRight { get; set; }
        [JsonPropertyName("bottomRight")]
        public LatLng BottomRight { get; set; }
        [JsonConstructor]
        public FourPointGeoArea() { }
        public FourPointGeoArea(LatLng topLeft, LatLng bottomLeft, LatLng topRight, LatLng bottomRight, Guid? id = null)
        {
            TopLeft = topLeft;
            BottomLeft = bottomLeft;
            TopRight = topRight;
            BottomRight = bottomRight;
        }
        public bool IsPointInside(LatLng point)
        {
            var polygon = new[] { TopLeft, BottomLeft, BottomRight, TopRight };

            int n = polygon.Length;
            bool isInside = false;

            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                if (((polygon[i].Longitude > point.Longitude) != (polygon[j].Longitude > point.Longitude)) &&
                    (point.Latitude < (polygon[j].Latitude - polygon[i].Latitude) * (point.Longitude - polygon[i].Longitude) / (polygon[j].Longitude - polygon[i].Longitude) + polygon[i].Latitude))
                {
                    isInside = true;
                }
            }

            return isInside;
        }
    }
}