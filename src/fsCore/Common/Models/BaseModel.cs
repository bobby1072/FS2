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
            for (var i = 0; i < allPropertiesToCheck.Length; i++)
            {
                var property = allPropertiesToCheck[i];
                if (property?.GetCustomAttribute<LockedProperty>() is not null && property.GetValue(this)?.Equals(property.GetValue(checkAgainst)) is false)
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
                var thisType = this.GetType().GetProperties();
                for (var i = 0; i < thisType.Length; i++)
                {
                    var property = thisType[i];
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
            var allProperties = GetType().GetProperties();
            for (var i = 0; i < allProperties.Length; i++)
            {
                var property = allProperties[i];
                var foundProp = property.GetValue(this);
                if (foundProp is BaseModel deepBaseModel)
                {
                    deepBaseModel.RemoveSensitive();
                }
                else if (foundProp is IEnumerable<BaseModel> deepBaseModels)
                {
                    deepBaseModels.RemoveSensitive();
                }
                else if (property?.GetCustomAttribute<SensitiveProperty>() is not null)
                {
                    property.SetValue(this, null);
                }
            }
        }
    }
    public static class BaseModelExtensionMethods
    {
        public static void RemoveSensitive<TModel>(this TModel model) where TModel : IEnumerable<BaseModel>
        {
            for (int i = 0; i < model.Count(); i++)
            {
                model.ElementAt(i).RemoveSensitive();
            }
        }
    }
}