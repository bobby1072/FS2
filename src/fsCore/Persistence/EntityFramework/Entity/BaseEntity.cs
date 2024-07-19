using Common.Models;

namespace Persistence.EntityFramework.Entity
{
    internal abstract record BaseEntity<TBase> where TBase : BaseModel
    {
        public abstract TBase ToRuntime();
    }
}