using System.Text.Json.Serialization;

namespace Common.Models
{
    public class GroupMember : BaseModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
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
        public Position? Position { get; set; }
        [JsonConstructor]
        public GroupMember(int id, Guid groupId, string userEmail, int positionId, User? user, Group? group, Position? position)
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