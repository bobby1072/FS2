using System.Text.Json;

namespace Common;
public static class Cloning
{
    public static T JsonClone<T>(this T source) where T : class
    {
        var serialisedObject = JsonSerializer.Serialize(source);
        return JsonSerializer.Deserialize<T>(serialisedObject) ?? throw new InvalidOperationException($"Failed to serialised {source.GetType().Name} instance");
    }
}