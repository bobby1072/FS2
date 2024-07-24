using System.Text.Json.Serialization;
using Common.Attributes;

namespace Common.Models
{
    public class GroupCatch : Catch
    {
        [LockedProperty]
        [JsonPropertyName("groupId")]
        public Guid GroupId { get; set; }
        [JsonPropertyName("group")]
        public Group? Group { get; set; }
        [JsonPropertyName("user")]
        public User? User { get; set; }
        [JsonPropertyName("catchPhoto")]
        public byte[]? CatchPhoto { get; set; }
        [JsonPropertyName("worldFish")]
        public WorldFish? WorldFish { get; set; }
        [JsonConstructor]
        public GroupCatch() { }
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
        public override bool ValidateAgainstOriginal<T>(T checkAgainst)
        {
            if (checkAgainst is not GroupCatch groupCatch)
            {
                return false;
            }
            else if (UserId != groupCatch.UserId || GroupId != groupCatch.GroupId || groupCatch.CreatedAt.Millisecond != CreatedAt.Millisecond)
            {
                return false;
            }
            else return true;
        }
    }
}