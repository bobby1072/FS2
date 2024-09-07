using System.Text.Json;

namespace Common.Misc;
public static class Cloning
{
    public static T JsonClone<T>(this T source) where T : class
    {
        var serialisedObject = JsonSerializer.Serialize(source);
        return JsonSerializer.Deserialize<T>(serialisedObject) ?? throw new InvalidOperationException($"Failed to serialised {source.GetType().Name} instance");
    }
    /// <summary>
    /// CLoning without json Warning: Unstable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T? DeepCopy<T>(this T obj)
    {
        if (obj == null)
            return default;

        var type = obj.GetType();
        if (type.IsPrimitive || type == typeof(string))
            return obj;

        var clone = Activator.CreateInstance(type);
        foreach (var field in type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
        {
            var fieldValue = field.GetValue(obj);
            field.SetValue(clone, fieldValue.DeepCopy());
        }

        return (T)clone;
    }
}