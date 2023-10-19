using System.Text.Json.Serialization;

namespace Common.Models
{
    public class GroupPosition : BaseModel
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }
        [JsonPropertyName("groupId")]
        public Guid GroupId { get; set; }
        [JsonPropertyName("group")]
        public Group? Group { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonConstructor]
        public GroupPosition(Guid groupId, string name, int? id, Group? group)
        {
            Id = id;
            GroupId = groupId;
            Group = group;
            Name = name;
        }
    }
}