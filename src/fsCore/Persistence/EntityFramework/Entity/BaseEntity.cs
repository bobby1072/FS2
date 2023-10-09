using Common.Models;

namespace Persistence.EntityFramework.Entity
{
    public abstract class BaseEntity<TBase> where TBase : BaseModel
    {
        public abstract TBase ToRuntime();
    }
}