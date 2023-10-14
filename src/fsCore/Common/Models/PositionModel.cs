using System.Text.Json.Serialization;

namespace Common.Models
{
    public class Position : BaseModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("groupId")]
        public Guid GroupId { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonConstructor]
        public Position(int id, Guid groupId, string name)
        {
            Id = id;
            GroupId = groupId;
            Name = name;
        }
    }
}