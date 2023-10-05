using Common.Models;

namespace Persistence.EntityFramework.Entity
{
    public abstract class BaseEntity
    {
        public abstract BaseModel ToRuntime();
    }
}