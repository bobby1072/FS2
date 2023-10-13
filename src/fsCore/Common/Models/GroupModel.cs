using System.Text.Json.Serialization;

namespace Common.Models
{
    public class Group : BaseModel
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("leaderEmail")]
        public string LeaderEmail { get; set; }
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("positions")]
        public ICollection<string> Positions { get; set; }
        [JsonPropertyName("public")]
        public bool Public { get; set; }
        [JsonPropertyName("listed")]
        public bool Listed { get; set; }
        [JsonPropertyName("emblem")]
        public byte[]? Emblem { get; set; }
        [JsonConstructor]
        public Group(string name, string leaderEmail, ICollection<string> positions, byte[]? emblem, string? description, Guid? id, DateTime? createdAt, bool? @public, bool? listed)
        {
            Id = id ?? Guid.NewGuid();
            Name = name;
            LeaderEmail = leaderEmail;
            CreatedAt = createdAt ?? DateTime.UtcNow;
            Positions = positions;
            Public = @public ?? false;
            Listed = listed ?? false;
            Emblem = emblem;
            Description = description;
        }
        public Group ApplyDefaults(string? leaderEmail)
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            Public = false;
            Listed = false;
            if (leaderEmail is not null && !string.IsNullOrEmpty(leaderEmail))
            {
                LeaderEmail = leaderEmail;
            }
            Positions = new List<string>();
            return this;
        }
    }
}