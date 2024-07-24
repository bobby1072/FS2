using System.Text.Json.Serialization;
using Common.Attributes;
namespace Common.Models
{
    public abstract class Catch : BaseModel
    {
        [LockedPropertyAttribute]
        [JsonPropertyName("id")]
        public Guid? Id { get; set; }
        [LockedPropertyAttribute]
        [JsonPropertyName("userId")]
        public Guid UserId { get; set; }
        [LockedPropertyAttribute]
        [JsonPropertyName("species")]
        public string Species { get; set; }
        [JsonPropertyName("weight")]
        public double Weight { get; set; }
        [JsonPropertyName("length")]
        public double Length { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [LockedPropertyAttribute]
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

    }
}