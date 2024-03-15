using System.Text.Json.Serialization;
using Common.Attributes;
namespace Common.Models
{
    public class GroupMember : BaseModel
    {
        [JsonConstructor]
        public GroupMember() { }
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
        [JsonPropertyName("positionId")]
        public Guid PositionId { get; set; }
        [JsonPropertyName("position")]
        public GroupPosition? Position { get; set; }
        public GroupMember(Guid groupId, Guid positionId, Guid userId, Guid? id = null, User? user = null, Group? group = null, GroupPosition? position = null)
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