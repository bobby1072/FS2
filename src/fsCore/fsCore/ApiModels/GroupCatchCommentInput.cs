using System.Text.Json.Serialization;

namespace fsCore.ApiModels
{
    public record GroupCatchCommentInput
    {
        [JsonPropertyName("id")]
        public int? Id { get; init; }
        [JsonPropertyName("comment")]
        public string Comment { get; init; }
        [JsonPropertyName("groupCatchId")]
        public Guid GroupCatchId { get; init; }
        [JsonPropertyName("userId")]
        public Guid UserId { get; init; }
        [JsonPropertyName("createdAt")]
        public DateTime? CreatedAt { get; init; }
    }
}