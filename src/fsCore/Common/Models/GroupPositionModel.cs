using System.Text.Json.Serialization;

namespace Common.Models
{
    public class GroupPosition : BaseModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("groupId")]
        public Guid GroupId { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonConstructor]
        public GroupPosition(int id, Guid groupId, string name)
        {
            Id = id;
            GroupId = groupId;
            Name = name;
        }
    }
}