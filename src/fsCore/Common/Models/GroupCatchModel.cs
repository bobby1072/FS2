using System.Text.Json.Serialization;
using Common.Attributes;

namespace Common.Models
{
    public class GroupCatch : BaseModel
    {
        [LockedProperty]
        [JsonPropertyName("id")]
        public Guid? Id { get; set; }
        [LockedProperty]
        [JsonPropertyName("groupId")]
        public Guid GroupId { get; set; }
        [JsonPropertyName("group")]
        public Group? Group { get; set; }
        [JsonPropertyName("userId")]
        public Guid UserId { get; set; }
        [JsonPropertyName("user")]
        public User? User { get; set; }
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
        [JsonPropertyName("catchPhoto")]
        public byte[]? CatchPhoto { get; set; }
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
        [JsonPropertyName("worldFishTaxocode")]
        public string? WorldFishTaxocode { get; set; }
        [JsonPropertyName("worldFish")]
        public WorldFish? WorldFish { get; set; }
        public GroupCatch(Guid userId, Guid groupId, string species, double weight, DateTime caughtAt, double length, double latitude, double longitude, string? description, Guid? id, DateTime? createdAt, byte[]? catchPhoto, Group? group, User? user, string? worldFishTaxocode, WorldFish? worldFish)
        {
            Id = id;
            Latitude = latitude;
            Longitude = longitude;
            UserId = userId;
            Species = species;
            Weight = weight;
            Length = length;
            WorldFishTaxocode = worldFishTaxocode;
            WorldFish = worldFish;
            Description = description;
            CreatedAt = createdAt ?? DateTime.UtcNow;
            CaughtAt = caughtAt;
            CatchPhoto = catchPhoto;
            GroupId = groupId;
            Group = group;
            User = user;
        }
        public GroupCatch ApplyDefaults(Guid? userId = null)
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            if (userId is Guid notNullUserId)
            {
                UserId = notNullUserId;
            }
            return this;
        }
    }
}