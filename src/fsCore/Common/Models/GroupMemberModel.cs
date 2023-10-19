using System.Text.Json.Serialization;

namespace Common.Models
{
    public class GroupMember : BaseModel
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }
        [JsonPropertyName("groupId")]
        public Guid GroupId { get; set; }
        [JsonPropertyName("group")]
        public Group? Group { get; set; }
        [JsonPropertyName("userEmail")]
        public string UserEmail { get; set; }
        [JsonPropertyName("user")]
        public User? User { get; set; }
        [JsonPropertyName("positionId")]
        public int PositionId { get; set; }
        [JsonPropertyName("position")]
        public GroupPosition? Position { get; set; }
        [JsonConstructor]
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