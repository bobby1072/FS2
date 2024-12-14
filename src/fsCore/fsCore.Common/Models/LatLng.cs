using System.Text.Json.Serialization;

namespace fsCore.Common.Models
{
    public class LatLng
    {
        [JsonPropertyName("latitude")]
        public decimal Latitude { get; set; }
        [JsonPropertyName("longitude")]
        public decimal Longitude { get; set; }
        [JsonConstructor]
        public LatLng(decimal latitude, decimal longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
        public override bool Equals(object? obj)
        {
            if (obj is not LatLng latLng)
            {
                return false;
            }
            return Latitude == latLng.Latitude && Longitude == latLng.Longitude;
        }
    }
}