using Microsoft.EntityFrameworkCore;

namespace Persistence.EntityFramework
{
    internal class FsContext: DbContext
    {
        public FsContext(DbContextOptions options): base(options) { }
        
    }
}
