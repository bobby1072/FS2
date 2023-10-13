using System.Text.Json.Serialization;

namespace Common.Models
{
    public class GroupModel : BaseModel
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
        public GroupModel(Guid id, string name, string leaderEmail, DateTime createdAt, ICollection<string> positions, bool @public, bool listed, byte[]? emblem, string? description)
        {
            Id = id;
            Name = name;
            LeaderEmail = leaderEmail;
            CreatedAt = createdAt;
            Positions = positions;
            Public = @public;
            Listed = listed;
            Emblem = emblem;
            Description = description;
        }
        public GroupModel ApplyDefaults(string? leaderEmail)
        {
            this.CreatedAt = DateTime.UtcNow;
            this.Public = false;
            this.Listed = false;
            if (leaderEmail is not null && !string.IsNullOrEmpty(leaderEmail))
            {
                this.LeaderEmail = leaderEmail;
            }
            this.Positions = new List<string>();
            return this;
        }
    }
}