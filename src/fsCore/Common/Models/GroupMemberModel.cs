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
        [JsonConstructor]
        public GroupMember(int id, Guid groupId, Group? group, string userEmail, User? user)
        {
            Id = id;
            GroupId = groupId;
            Group = group;
            UserEmail = userEmail;
            User = user;
        }
    }
}