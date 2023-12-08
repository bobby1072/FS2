using System.Text.Json.Serialization;

namespace Common.Models
{
    public class GroupCatch : BaseModel
    {
        [JsonPropertyName("id")]
        public Guid? Id { get; set; }
        [JsonPropertyName("groupId")]
        public Guid GroupId { get; set; }
        [JsonPropertyName("group")]
        public Group? Group { get; set; }
        [JsonPropertyName("userEmail")]
        public string UserEmail { get; set; }
        [JsonPropertyName("user")]
        public User? User { get; set; }
        [JsonPropertyName("species")]
        public string Species { get; set; }
        [JsonPropertyName("weight")]
        public double Weight { get; set; }
        [JsonPropertyName("length")]
        public double Length { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
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
        [JsonConstructor]
        public GroupCatch(string userEmail, Guid groupId, string species, double weight, DateTime caughtAt, double length, double latitude, double longitude, string? description, Guid? id, DateTime? createdAt, byte[]? catchPhoto, Group? group, User? user)
        {
            Id = id;
            UserEmail = userEmail;
            Species = species;
            Weight = weight;
            Length = length;
            Description = description;
            CreatedAt = createdAt ?? DateTime.UtcNow;
            CaughtAt = caughtAt;
            CatchPhoto = catchPhoto;
            GroupId = groupId;
            Group = group;
            User = user;
        }
        public GroupCatch ApplyDefaults()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            return this;
        }
    }
}