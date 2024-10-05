using System.Text.Json.Serialization;

namespace fsCore.ApiModels
{
    public record GroupCatchCommentInput
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }
        [JsonPropertyName("comment")]
        public string Comment { get; set; }
        [JsonPropertyName("groupCatchId")]
        public Guid GroupCatchId { get; set; }
        [JsonPropertyName("userId")]
        public Guid UserId { get; set; }
        [JsonPropertyName("createdAt")]
        public DateTime? CreatedAt { get; set; }
    }
}