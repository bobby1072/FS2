using Common.Attributes;
using System.Text.Json.Serialization;
namespace Common.Models
{
    public abstract class Catch : BaseModel
    {
        [LockedProperty]
        [JsonPropertyName("id")]
        public Guid? Id { get; set; }
        [LockedProperty]
        [JsonPropertyName("userId")]
        public Guid UserId { get; set; }
        [LockedProperty]
        [JsonPropertyName("species")]
        public string Species { get; set; }
        [JsonPropertyName("weight")]
        public double Weight { get; set; }
        [JsonPropertyName("length")]
        public double Length { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [LockedProperty]
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("caughtAt")]
        public DateTime CaughtAt { get; set; }
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
        [JsonPropertyName("worldFishTaxocode")]
        public string? WorldFishTaxocode { get; set; }
        public override bool Equals(object? obj)
        {
            if (obj is not Catch catchObj)
            {
                return false;
            }
            return Id == catchObj.Id
            && UserId == catchObj.UserId
            && Species == catchObj.Species
            && Weight == catchObj.Weight
            && Length == catchObj.Length
            && Description == catchObj.Description
            && CreatedAt == catchObj.CreatedAt
            && CaughtAt == catchObj.CaughtAt
            && Latitude == catchObj.Latitude
            && Longitude == catchObj.Longitude
            && WorldFishTaxocode == catchObj.WorldFishTaxocode;
        }

    }
}