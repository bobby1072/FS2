using Common.Attributes;
using System.Text.Json.Serialization;
namespace Common.Models
{
    public class GroupPosition : BaseModel
    {
        [LockedProperty]
        [JsonPropertyName("id")]
        public int? Id { get; set; }
        [LockedProperty]
        [JsonPropertyName("groupId")]
        public Guid GroupId { get; set; }
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
            bool canManageMembers = true)
        {
            Id = id;
            GroupId = groupId;
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