using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;

namespace Persistence.EntityFramework
{
    internal class FsContext : DbContext
    {
        public FsContext(DbContextOptions options) : base(options) { }
        public virtual DbSet<WorldFishEntity> WorldFish { get; set; }
        public virtual DbSet<UserEntity> User { get; set; }
    }
}
