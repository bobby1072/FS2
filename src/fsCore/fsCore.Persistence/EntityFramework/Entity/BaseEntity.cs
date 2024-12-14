using fsCore.Common.Models;

namespace fsCore.Persistence.EntityFramework.Entity
{
    internal abstract record BaseEntity<TBase> where TBase : BaseModel
    {
        public virtual TBase ToRuntime()
        {
            throw new NotImplementedException();
        }
    }
}