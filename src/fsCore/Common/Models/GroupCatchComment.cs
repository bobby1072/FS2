using Common.Attributes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Common.Models
{
    public class GroupCatchComment : BaseModel
    {
        public GroupCatchComment ApplyDefaults(Guid? userId = null)
        {
            CreatedAt = DateTime.UtcNow;
            UserId = userId ?? UserId;
            return this;
        }
        [LockedPropertyAttribute]
        [JsonPropertyName("id")]
        public int? Id { get; set; }
        [LockedPropertyAttribute]
        [JsonPropertyName("groupCatchId")]
        public Guid GroupCatchId { get; set; }
        [LockedPropertyAttribute]
        [JsonPropertyName("userId")]
        public Guid UserId { get; set; }
        public User? User { get; set; }
        [JsonPropertyName("comment")]
        public string Comment { get; set; }
        [LockedPropertyAttribute]
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("taggedUsers")]
        public ICollection<GroupCatchCommentTaggedUsers>? TaggedUsers { get; set; }
        public GroupCatchComment(int? id, Guid groupCatchId, Guid userId, string comment, DateTime createdAt, ICollection<GroupCatchCommentTaggedUsers>? taggedUsers = null, User? user = null)
        {
            Id = id;
            GroupCatchId = groupCatchId;
            User = user;
            UserId = userId;
            Comment = comment;
            CreatedAt = createdAt;
            TaggedUsers = taggedUsers;
        }
    }
    public class GroupCatchCommentTaggedUsers : BaseModel
    {
        public GroupCatchCommentTaggedUsers(int commentId, Guid userId)
        {
            CommentId = commentId;
            UserId = userId;
        }
        public GroupCatchCommentTaggedUsers(int id, int commentId, Guid userId, User? user = null)
        {
            Id = id;
            CommentId = commentId;
            UserId = userId;
            User = user;
        }
        [JsonPropertyName("id")]
        [LockedPropertyAttribute]
        public int? Id { get; set; }
        [JsonPropertyName("commentId")]
        [LockedPropertyAttribute]
        public int CommentId { get; set; }
        [JsonPropertyName("userId")]
        [LockedPropertyAttribute]
        public Guid UserId { get; set; }
        [JsonPropertyName("user")]
        public User? User { get; set; }
    }
    public static partial class GroupCatchCommentUtils
    {
        [GeneratedRegex("@([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})")]
        private static partial Regex RegexPatternGenerator();
        public static Regex RegexPattern = RegexPatternGenerator();
        public const int MaximumTags = 20;
    }
}