using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Models;

namespace Persistence.EntityFramework.Entity
{
    [Table("group_catch_comment", Schema = DbConstants.PublicSchema)]
    internal record GroupCatchCommentEntity : BaseEntity<GroupCatchComment>
    {
        [Key]
        [Column("id")]
        public int Id { get; init; }
        [Column("comment")]
        public string Comment { get; set; }
        [Column("catch_id")]
        public Guid CatchId { get; set; }
        [Column("user_id")]
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public UserEntity? User { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<GroupCatchCommentTaggedUsersEntity>? TaggedUsers { get; set; }
        public override GroupCatchComment ToRuntime()
        {
            return new GroupCatchComment(Id, CatchId, UserId, Comment, CreatedAt, TaggedUsers?.Select(x => x.ToRuntime()).ToArray(), User?.ToRuntime());
        }
        public static GroupCatchCommentEntity RuntimeToEntity(GroupCatchComment groupCatchComment)
        {
            if (groupCatchComment.Id is not null)
            {
                return new GroupCatchCommentEntity
                {
                    Comment = groupCatchComment.Comment,
                    CatchId = groupCatchComment.GroupCatchId,
                    UserId = groupCatchComment.UserId,
                    CreatedAt = groupCatchComment.CreatedAt,
                    Id = groupCatchComment.Id!.Value
                };
            }
            else
            {
                return new GroupCatchCommentEntity
                {
                    Comment = groupCatchComment.Comment,
                    CatchId = groupCatchComment.GroupCatchId,
                    UserId = groupCatchComment.UserId,
                    CreatedAt = groupCatchComment.CreatedAt,
                };

            }
        }
    }
    [Table("group_catch_comment_tagged_users", Schema = DbConstants.PublicSchema)]
    internal record GroupCatchCommentTaggedUsersEntity : BaseEntity<GroupCatchCommentTaggedUsers>
    {
        [Key]
        [Column("id")]
        public int Id { get; init; }
        [Column("comment_id")]
        public int CommentId { get; init; }
        [Column("user_id")]
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public UserEntity? User { get; set; }
        public override GroupCatchCommentTaggedUsers ToRuntime()
        {
            return new GroupCatchCommentTaggedUsers(Id, CommentId, UserId, User?.ToRuntime());
        }
        public static GroupCatchCommentTaggedUsersEntity RuntimeToEntity(GroupCatchCommentTaggedUsers groupCatchCommentTaggedUsers)
        {
            if (groupCatchCommentTaggedUsers.Id is not null)
            {
                return new GroupCatchCommentTaggedUsersEntity { CommentId = groupCatchCommentTaggedUsers.CommentId, UserId = groupCatchCommentTaggedUsers.UserId, Id = groupCatchCommentTaggedUsers.Id!.Value };
            }
            else
            {
                return new GroupCatchCommentTaggedUsersEntity { CommentId = groupCatchCommentTaggedUsers.CommentId, UserId = groupCatchCommentTaggedUsers.UserId };
            }
        }
    }
}