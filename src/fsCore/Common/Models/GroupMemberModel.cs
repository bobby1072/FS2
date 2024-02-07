using System.Text.Json.Serialization;
using Common.Attributes;
namespace Common.Models
{
    public class GroupMember : BaseModel
    {
        [LockedProperty]
        [JsonPropertyName("id")]
        public int? Id { get; set; }
        [LockedProperty]
        [JsonPropertyName("groupId")]
        public Guid GroupId { get; set; }
        [JsonPropertyName("group")]
        public Group? Group { get; set; }
        [LockedProperty]
        [JsonPropertyName("userEmail")]
        public string UserEmail { get; set; }
        [JsonPropertyName("user")]
        public User? User { get; set; }
        [JsonPropertyName("positionId")]
        public int PositionId { get; set; }
        [JsonPropertyName("position")]
        public GroupPosition? Position { get; set; }
        public GroupMember(Guid groupId, string userEmail, int positionId, int? id = null, User? user = null, Group? group = null, GroupPosition? position = null)
        {
            Id = id;
            PositionId = positionId;
            Position = position;
            GroupId = groupId;
            Group = group;
            UserEmail = userEmail;
            User = user;
        }
    }
}