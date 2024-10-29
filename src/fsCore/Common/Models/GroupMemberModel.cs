using Common.Attributes;
using System.Text.Json.Serialization;
namespace Common.Models
{
    public class GroupMember : BaseModel
    {
        [JsonConstructor]
        public GroupMember() { }
        [LockedProperty]
        [JsonPropertyName("id")]
        public int? Id { get; set; }
        [LockedProperty]
        [JsonPropertyName("groupId")]
        public Guid GroupId { get; set; }
        [JsonPropertyName("group")]
        public Group? Group { get; set; }
        [JsonPropertyName("userId")]
        public Guid UserId { get; set; }
        [JsonPropertyName("user")]
        public User? User { get; set; }
        [JsonPropertyName("positionId")]
        public int PositionId { get; set; }
        [JsonPropertyName("position")]
        public GroupPosition? Position { get; set; }
        public GroupMember(Guid groupId, int positionId, Guid userId, int? id = null, User? user = null, Group? group = null, GroupPosition? position = null)
        {
            Id = id;
            PositionId = positionId;
            Position = position;
            GroupId = groupId;
            UserId = userId;
            Group = group;
            User = user;
        }
    }
}