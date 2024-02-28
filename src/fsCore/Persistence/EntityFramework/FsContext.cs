using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;

namespace Persistence.EntityFramework
{
    internal class FsContext : DbContext
    {
        public FsContext(DbContextOptions options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorldFishEntity>()
                .HasKey(x => x.Taxocode);
            modelBuilder.Entity<UserEntity>()
                .HasKey(x => x.Username);
            modelBuilder.Entity<GroupEntity>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<GroupEntity>()
                .HasOne(g => g.Leader)
                .WithMany()
                .HasForeignKey(g => g.LeaderUsername);
            modelBuilder.Entity<GroupEntity>()
                .HasMany(g => g.Members)
                .WithOne(gm => gm.Group)
                .HasForeignKey(gm => gm.GroupId);
            modelBuilder.Entity<GroupEntity>()
                .HasMany(g => g.Positions)
                .WithOne(gp => gp.Group)
                .HasForeignKey(gp => gp.GroupId);
            modelBuilder.Entity<GroupEntity>()
                .HasMany(g => g.Catches)
                .WithOne(gc => gc.Group)
                .HasForeignKey(gc => gc.GroupId);
        }
        public virtual DbSet<WorldFishEntity> WorldFish { get; set; }
        public virtual DbSet<UserEntity> User { get; set; }
        public virtual DbSet<GroupEntity> Group { get; set; }
        public virtual DbSet<GroupPositionEntity> Position { get; set; }
        public virtual DbSet<GroupMemberEntity> GroupMember { get; set; }

    }
}
