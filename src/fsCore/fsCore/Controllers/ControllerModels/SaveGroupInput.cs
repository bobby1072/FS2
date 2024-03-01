using System.Text.Json.Serialization;
using Common.Models;

namespace fsCore.Controllers.ControllerModels
{
    public class SaveGroupInput
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("leaderId")]
        public Guid LeaderId { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("isPublic")]
        public bool IsPublic { get; set; }
        [JsonPropertyName("isListed")]
        public bool IsListed { get; set; }
        [JsonPropertyName("emblem")]
        public string? Emblem { get; set; }
        [JsonPropertyName("createdAt")]
        public string? CreatedAt { get; set; }
        public Group ToGroup()
        {
            return new Group(
                Name,
                Emblem is not null ? Convert.FromBase64String(Emblem) : null,
                Description,
                Id is not null ? Guid.Parse(Id) : null,
                CreatedAt is not null ? DateTime.Parse(CreatedAt) : DateTime.Now,
                IsPublic,
                IsListed,
                LeaderId
            );
        }
    }
}