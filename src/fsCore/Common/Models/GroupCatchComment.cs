namespace Common.Models
{
    public class GroupCatchComment : BaseModel
    {
        public GroupCatchComment ApplyDefaults()
        {
            CreatedAt = DateTime.UtcNow;
            return this;
        }
        public int? Id { get; set; }
        public Guid GroupCatchId { get; set; }
        public Guid UserId { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public GroupCatchComment(int? id, Guid groupCatchId, Guid userId, string comment, DateTime createdAt)
        {
            Id = id;
            GroupCatchId = groupCatchId;
            UserId = userId;
            Comment = comment;
            CreatedAt = createdAt;
        }
    }
}