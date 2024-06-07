using System.Text.Json.Serialization;
using Common.Models;

namespace fsCore.Controllers.ControllerModels
{
    public class GroupCatchCommentInput
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
        public GroupCatchComment ToGroupCatchComment()
        {
            return new GroupCatchComment(Id, GroupCatchId, UserId, Comment, CreatedAt ?? DateTime.UtcNow);
        }
    }
}