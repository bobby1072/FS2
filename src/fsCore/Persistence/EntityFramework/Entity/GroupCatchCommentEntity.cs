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
        public DateTime CreatedAt { get; set; }
        public override GroupCatchComment ToRuntime()
        {
            return new GroupCatchComment(Id, GroupCatchId, UserId, Comment, CreatedAt);
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
}