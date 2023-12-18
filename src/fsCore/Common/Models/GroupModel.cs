using System.Text.Json.Serialization;

namespace Common.Models
{
    public class Group : BaseModel
    {
        [JsonPropertyName("id")]
        public Guid? Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("leaderEmail")]
        public string LeaderEmail { get; set; }
        [JsonPropertyName("leader")]
        public User? Leader { get; set; }
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("public")]
        public bool Public { get; set; }
        [JsonPropertyName("listed")]
        public bool Listed { get; set; }
        [JsonPropertyName("emblem")]
        public byte[]? Emblem { get; set; }
        [JsonPropertyName("members")]
        public ICollection<GroupMember>? Members { get; set; }
        [JsonPropertyName("positions")]
        public ICollection<GroupPosition>? Positions { get; set; }
        [JsonPropertyName("catches")]
        public ICollection<GroupCatch>? Catches { get; set; }
        public Group(string name, string leaderEmail, byte[]? emblem, string? description, Guid? id, DateTime? createdAt, bool? @public, bool? listed, User? leader, ICollection<GroupMember>? members, ICollection<GroupPosition>? positions, ICollection<GroupCatch>? catches)
        {
            Positions = positions;
            Id = id;
            Catches = catches;
            Name = name;
            Leader = leader;
            LeaderEmail = leaderEmail;
            CreatedAt = createdAt ?? DateTime.UtcNow;
            Public = @public ?? false;
            Listed = listed ?? false;
            Emblem = emblem;
            Description = description;
            Members = members;
        }
        public Group ApplyDefaults(string? leaderEmail)
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            if (leaderEmail is not null && !string.IsNullOrEmpty(leaderEmail))
            {
                LeaderEmail = leaderEmail;
            }
            return this;
        }
    }
}