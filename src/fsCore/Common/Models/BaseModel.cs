using System.Reflection;
using Common.Attributes;

namespace Common.Models
{
    public abstract class BaseModel
    {
        public virtual bool Validate<TModel>(TModel checkAgainst) where TModel : BaseModel
        {
            if (this.GetType() != typeof(TModel))
            {
                return false;
            }
            var allPropertiesToCheck = checkAgainst.GetType().GetProperties();
            foreach (var property in allPropertiesToCheck)
            {
                if (property.GetCustomAttribute<LockedProperty>() is not null && property.GetValue(this) != property.GetValue(checkAgainst))
                {
                    return false;
                }
            }
            return true;
        }
    }
}