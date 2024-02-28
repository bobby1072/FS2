using System.Reflection;
using Common.Attributes;

namespace Common.Models
{
    public abstract class BaseModel
    {
        public virtual bool ValidateAgainstOriginal<TModel>(TModel checkAgainst) where TModel : BaseModel
        {
            if (this is not TModel)
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
        public override bool Equals(object? obj)
        {
            if (obj is not null)
            {
                foreach (var property in this.GetType().GetProperties())
                {
                    var foundSelfValue = property.GetValue(this);
                    var foundObjVal = property.GetValue(obj);
                    if (foundSelfValue != foundObjVal)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        public virtual void RemoveSensitive()
        {
            var allProperties = this.GetType().GetProperties();
            foreach (var property in allProperties)
            {
                if (property.GetCustomAttribute<SensitiveProperty>() is not null)
                {
                    property.SetValue(this, null);
                }
            }
        }
    }
}