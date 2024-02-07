using System.Reflection;
using System.Text.Json.Serialization;
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
                if (property.GetCustomAttribute<LockedProperty>() is not null && property.GetValue(this)?.Equals(property.GetValue(checkAgainst)) is false)
                {
                    return false;
                }
            }
            return true;
        }
    }
}