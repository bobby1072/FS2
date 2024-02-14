using System.Net;
using System.Text.Json.Serialization;
using Common.Attributes;
using Common.Utils;
namespace Common.Models
{
    public class Group : BaseModel
    {
        [LockedProperty]
        [JsonPropertyName("id")]
        public Guid? Id { get; set; }
        private string _name;
        [JsonPropertyName("name")]
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrEmpty(value) || value == " " || value.IsJustNumbers() || value.IsJustSpaces())
                {
                    throw new ApiException(ErrorConstants.GroupNameCorrectFormat, HttpStatusCode.UnprocessableEntity);
                }
                _name = value;
            }
        }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [LockedProperty]
        [JsonPropertyName("leaderUsername")]
        public string LeaderUsername { get; set; }
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
        public Group(string name, string leaderUsername, byte[]? emblem, string? description, Guid? id, DateTime? createdAt, bool? @public, bool? listed, User? leader, ICollection<GroupMember>? members, ICollection<GroupPosition>? positions, ICollection<GroupCatch>? catches)
        {
            Positions = positions;
            Id = id;
            Catches = catches;
            Name = name;
            Leader = leader;
            LeaderUsername = leaderUsername;
            CreatedAt = createdAt ?? DateTime.UtcNow;
            Public = @public ?? false;
            Listed = listed ?? false;
            Emblem = emblem;
            Description = description;
            Members = members;
        }
        public Group ApplyDefaults(string? leaderUsername)
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            if (leaderUsername is not null && !string.IsNullOrEmpty(leaderUsername))
            {
                LeaderUsername = leaderUsername;
            }
            return this;
        }
        public Group() { }
        public override bool Equals(object? obj)
        {
            if (obj is Group group)
            {
                return group.Id == Id
                && group.Name == Name
                && group.Description == Description
                && group.LeaderUsername == LeaderUsername
                && group.CreatedAt == CreatedAt
                && group.Public == Public
                && group.Listed == Listed;
            }
            return false;
        }
    }
}