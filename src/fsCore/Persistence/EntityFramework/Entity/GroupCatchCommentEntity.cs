using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Models;

namespace Persistence.EntityFramework.Entity
{
    [Table("group_catch_comment", Schema = DbConstants.MainSchema)]
    internal class GroupCatchCommentEntity : BaseEntity<GroupCatchComment>
    {
        [Key]
        public int Id { get; set; }
        public string Comment { get; set; }
        public Guid GroupCatchId { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public UserEntity? User { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<GroupCatchCommentTaggedUsersEntity>? TaggedUsers { get; set; }
        public override GroupCatchComment ToRuntime()
        {
            return new GroupCatchComment(Id, GroupCatchId, UserId, Comment, CreatedAt, TaggedUsers?.Select(x => x.ToRuntime()).ToArray(), User?.ToRuntime());
        }
        public static GroupCatchCommentEntity RuntimeToEntity(GroupCatchComment groupCatchComment)
        {
            var ent = new GroupCatchCommentEntity
            {
                Comment = groupCatchComment.Comment,
                GroupCatchId = groupCatchComment.GroupCatchId,
                UserId = groupCatchComment.UserId,
                CreatedAt = groupCatchComment.CreatedAt
            };
            if (groupCatchComment.Id.HasValue)
                ent.Id = groupCatchComment.Id.Value;
            return ent;
        }
    }
    [Table("group_catch_comment_tagged_users", Schema = DbConstants.MainSchema)]
    internal class GroupCatchCommentTaggedUsersEntity : BaseEntity<GroupCatchCommentTaggedUsers>
    {
        public GroupCatchCommentTaggedUsersEntity(int commentId, Guid userId)
        {
            CommentId = commentId;
            UserId = userId;
        }
        public int CommentId { get; set; }
        public Guid UserId { get; set; }
        public override GroupCatchCommentTaggedUsers ToRuntime()
        {
            return new GroupCatchCommentTaggedUsers(CommentId, UserId);
        }
        public static GroupCatchCommentTaggedUsersEntity RuntimeToEntity(GroupCatchCommentTaggedUsers groupCatchCommentTaggedUsers)
        {
            return new GroupCatchCommentTaggedUsersEntity(groupCatchCommentTaggedUsers.CommentId, groupCatchCommentTaggedUsers.UserId);
        }
    }
}