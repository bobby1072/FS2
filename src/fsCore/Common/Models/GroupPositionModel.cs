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
        [JsonPropertyName("canManageGroup")]
        public bool CanManageGroup { get; set; }
        [JsonPropertyName("canReadCatches")]
        public bool CanReadCatches { get; set; }
        [JsonPropertyName("canManageCatches")]
        public bool CanManageCatches { get; set; }
        [JsonPropertyName("canReadMembers")]
        public bool CanReadMembers { get; set; }
        [JsonPropertyName("canManageMembers")]
        public bool CanManageMembers { get; set; }
        public GroupPosition(
            Guid groupId,
            string name,
            int? id = null,
            bool canManageGroup = false,
            bool canReadCatches = true,
            bool canManageCatches = false,
            bool canReadMembers = true,
            bool canManageMembers = true,
            Group? group = null)
        {
            Id = id;
            GroupId = groupId;
            Group = group;
            Name = name;
            CanManageGroup = canManageGroup;
            CanReadCatches = canReadCatches;
            CanManageCatches = canManageCatches;
            CanReadMembers = canReadMembers;
            CanManageMembers = canManageMembers;
        }
        public GroupPosition ApplyDefaults()
        {
            CanManageGroup = false;
            CanReadCatches = true;
            CanManageCatches = false;
            CanReadMembers = true;
            CanManageMembers = false;
            return this;
        }
    }
}