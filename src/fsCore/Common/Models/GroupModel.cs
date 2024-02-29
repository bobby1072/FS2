using System.Text.Json.Serialization;
using Common.Attributes;
using Common.Models.Validators;
using FluentValidation;
namespace Common.Models
{
    public class Group : BaseModel
    {
        private static readonly GroupValidator _validator = new();
        [LockedProperty]
        [JsonPropertyName("id")]
        public Guid? Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("leaderId")]
        public Guid LeaderId { get; set; }
        [JsonPropertyName("leader")]
        public User? Leader { get; set; }
        [LockedProperty]
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
        public Group(string name, byte[]? emblem, string? description, Guid? id, DateTime? createdAt, bool? @public, bool? listed, Guid leaderId, User? leader = null, ICollection<GroupMember>? members = null, ICollection<GroupPosition>? positions = null, ICollection<GroupCatch>? catches = null)
        {
            Positions = positions;
            Id = id;
            Catches = catches;
            Name = name;
            Leader = leader;
            LeaderId = leaderId;
            CreatedAt = createdAt ?? DateTime.UtcNow;
            Public = @public ?? false;
            Listed = listed ?? false;
            Emblem = emblem;
            Description = description;
            Members = members;
            _validator.ValidateAndThrow(this);
        }
        public Group ApplyDefaults(Guid? leaderId)
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            if (leaderId is Guid notNullLeaderId)
            {
                LeaderId = notNullLeaderId;
            }
            return this;
        }
        [JsonConstructor]
        public Group() { }
        public override bool Equals(object? obj)
        {
            if (obj is Group group)
            {
                return group.Id == Id
                && group.Name == Name
                && group.Description == Description
                && group.LeaderId == LeaderId
                && group.CreatedAt == CreatedAt
                && group.Public == Public
                && group.Listed == Listed;
            }
            return false;
        }
    }
}