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
        public int? Id { get; set; }
        public Guid GroupCatchId { get; set; }
        public Guid UserId { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<GroupCatchCommentTaggedUsers>? TaggedUsers { get; set; }
        public GroupCatchComment(int? id, Guid groupCatchId, Guid userId, string comment, DateTime createdAt, ICollection<GroupCatchCommentTaggedUsers>? taggedUsers = null)
        {
            Id = id;
            GroupCatchId = groupCatchId;
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
        public int CommentId { get; set; }
        public Guid UserId { get; set; }
    }
    public static partial class GroupCatchCommentUtils
    {
        [GeneratedRegex("@([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})")]
        private static partial Regex RegexPatternGenerator();
        public static Regex RegexPattern = RegexPatternGenerator();
        public const int MaximumTags = 20;
    }
}