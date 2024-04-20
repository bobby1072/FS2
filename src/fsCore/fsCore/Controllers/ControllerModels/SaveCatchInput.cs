using System.Text.Json.Serialization;
using Common.Models;

namespace fsCore.Controllers.ControllerModels
{
    public class SaveCatchInput
    {
        [JsonPropertyName("id")]
        public Guid? Id { get; set; }
        [JsonPropertyName("groupId")]
        public Guid GroupId { get; set; }
        [JsonPropertyName("species")]
        public string Species { get; set; }
        [JsonPropertyName("weight")]
        public double Weight { get; set; }
        [JsonPropertyName("length")]
        public double Length { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("caughtAt")]
        public DateTime CaughtAt { get; set; }
        [JsonPropertyName("catchPhoto")]
        public string? CatchPhoto { get; set; }
        [JsonPropertyName("createdAt")]
        public string? CreatedAt { get; set; }
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
        [JsonPropertyName("worldFishTaxocode")]
        public string? WorldFishTaxocode { get; set; }
        public GroupCatch ToGroupCatch(Guid userId)
        {
            return new GroupCatch(
                userId: userId,
                groupId: GroupId,
                species: Species,
                weight: Weight,
                caughtAt: CaughtAt,
                length: Length,
                latitude: Latitude,
                longitude: Longitude,
                description: Description,
                id: Id,
                createdAt: CreatedAt is not null ? DateTime.Parse(CreatedAt).ToUniversalTime() : DateTime.UtcNow,
                catchPhoto: CatchPhoto is not null ? Convert.FromBase64String(CatchPhoto) : null,
                group: null,
                user: null,
                worldFishTaxocode: WorldFishTaxocode,
                worldFish: null
            );
        }
    }
}