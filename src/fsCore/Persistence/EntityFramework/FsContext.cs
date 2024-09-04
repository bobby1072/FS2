using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;

namespace Persistence.EntityFramework
{
    internal class FsContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GroupCatchCommentEntity>()
                .HasMany(c => c.TaggedUsers)
                .WithOne()
                .HasForeignKey(tu => tu.CommentId);
            modelBuilder.Entity<GroupCatchCommentTaggedUsersEntity>()
                .HasOne(tu => tu.User)
                .WithOne()
                .HasForeignKey<GroupCatchCommentTaggedUsersEntity>(tu => tu.UserId);
        }
        public FsContext(DbContextOptions options) : base(options) { }
        public virtual DbSet<WorldFishEntity> WorldFish { get; set; }
        public virtual DbSet<GroupCatchCommentTaggedUsersEntity> CommentTaggedUsers { get; set; }
        public virtual DbSet<UserEntity> User { get; set; }
        public virtual DbSet<GroupCatchCommentEntity> GroupCatchComment { get; set; }
        public virtual DbSet<GroupEntity> Group { get; set; }
        public virtual DbSet<GroupPositionEntity> Position { get; set; }
        public virtual DbSet<GroupMemberEntity> GroupMember { get; set; }
        public virtual DbSet<GroupCatchEntity> GroupCatch { get; set; }
        public virtual DbSet<ActiveLiveMatchCatchEntity> ActiveLiveMatchCatch { get; set; }
        public virtual DbSet<ActiveLiveMatchEntity> ActiveLiveMatch { get; set; }
        public virtual DbSet<ActiveLiveMatchParticipantEntity> ActiveLiveMatchParticipant { get; set; }
    }
}
