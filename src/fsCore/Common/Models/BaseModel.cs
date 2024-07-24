using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Common.Attributes;
using Common.Utils;

namespace Common.Models
{
    /// <summary>
    /// <para>General base model for all models.</para>
    /// </summary>
    public abstract class BaseModel
    {
        private static readonly Type[] _allBaseModelChildren = CommonAssemblyUtils.AllAssemblyTypes.Where(t => t.IsSubclassOf(typeof(BaseModel))).ToArray();
        public static T ParseToChildOf<T>(object? obj) where T : BaseModel
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            var targetType = typeof(T);
            var childClasses = _allBaseModelChildren.Where(x => x.IsSubclassOf(targetType)).ToArray();
            foreach (var childType in childClasses)
            {
                try
                {
                    var objConstructor = childType.GetConstructors().FirstOrDefault(x => x.GetCustomAttribute<AssemblyConstructor>() is not null) ?? throw new InvalidDataException("No assembly constructor found");
                    var parsedObj = objConstructor.Invoke(new object[] { obj });
                    if (parsedObj is not null)
                    {
                        return (parsedObj as T)!;
                    }
                }
                catch (Exception) { }
            }
            throw new InvalidCastException($"Cannot cast object");
        }
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
    /// <summary>
    /// <para>STRICTLY TEST PURPOSES ONLY</para>
    /// <para>Test base model for vehicle.</para>
    /// </summary>
    public class TestBaseModelVehicle : BaseModel
    {
        [JsonPropertyName("manufacturer")]
        public string Manufacturer { get; set; }
        [JsonPropertyName("year")]
        public int Year { get; set; }
        // public TestBaseModelVehicle(string manufacturer, int year)
        // {
        //     Manufacturer = manufacturer;
        //     Year = year;
        // }
    }
    /// <summary>
    /// <para>STRICTLY TEST PURPOSES ONLY</para>
    /// <para>Test base model for car.</para>
    /// </summary>
    public class TestBaseModelCar : TestBaseModelVehicle
    {
        public TestBaseModelCar() { }
        [JsonPropertyName("driveSystem")]
        public string DriveSystem { get; set; }
        // public TestBaseModelCar(string manufacturer, int year, string driveSystem) : base(manufacturer, year)
        // {
        //     DriveSystem = driveSystem;
        // }
        [AssemblyConstructor]
        public TestBaseModelCar(object? obj)
        {
            if (obj is null)
            {
                throw new InvalidDataException("Object is null");
            }
            else if (obj is JsonElement jsonElement)
            {
                Manufacturer = jsonElement.GetProperty("manufacturer").GetString();
                Year = jsonElement.GetProperty("year").GetInt32();
                DriveSystem = jsonElement.GetProperty("driveSystem").GetString();
                return;
            }
            else if (obj is TestBaseModelCar testBaseModelCar)
            {
                Manufacturer = testBaseModelCar.Manufacturer;
                Year = testBaseModelCar.Year;
                DriveSystem = testBaseModelCar.DriveSystem;
                return;
            }
            throw new InvalidDataException("Object is not a valid type");
        }
    }
    /// <summary>
    /// <para>STRICTLY TEST PURPOSES ONLY</para>
    /// <para>Test base model for truck.</para>
    /// </summary>
    public class TestBaseModelTruck : TestBaseModelVehicle
    {
        public TestBaseModelTruck() { }
        [JsonPropertyName("cargoType")]
        public string CargoType { get; set; }
        // public TestBaseModelTruck(string manufacturer, int year, string cargoType) : base(manufacturer, year)
        // {
        //     CargoType = cargoType;
        // }
        [AssemblyConstructor]
        public TestBaseModelTruck(object? obj)
        {
            if (obj is null)
            {
                throw new InvalidDataException("Object is null");
            }
            else if (obj is JsonElement jsonElement)
            {
                Manufacturer = jsonElement.GetProperty("manufacturer").GetString();
                Year = jsonElement.GetProperty("year").GetInt32();
                CargoType = jsonElement.GetProperty("cargoType").GetString();
                return;
            }
            else if (obj is TestBaseModelTruck testBaseModelTruck)
            {
                Manufacturer = testBaseModelTruck.Manufacturer;
                Year = testBaseModelTruck.Year;
                CargoType = testBaseModelTruck.CargoType;
                return;
            }
            throw new InvalidDataException("Object is not a valid type");
        }
    }
}