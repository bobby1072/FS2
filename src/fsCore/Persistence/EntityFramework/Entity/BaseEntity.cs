using Common.Models;

namespace Persistence.EntityFramework.Entity
{
    internal abstract class BaseEntity<TBase> where TBase : BaseModel
    {
        public abstract TBase ToRuntime();
    }
}