using System.Reflection;
using System.Text.Json;

namespace Common.Utils
{
    public static class AssemblyUtils
    {
        public static readonly Assembly RuntimeAssembly = Assembly.GetExecutingAssembly();
        public static readonly IReadOnlyCollection<Type> AllAssemblyTypes = RuntimeAssembly.GetTypes();
        public static T? TryParseToChildOf<T>(string jsonObj) where T : class
        {
            try
            {
                return ParseToChildOf<T>(jsonObj);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static T ParseToChildOf<T>(string jsonObj) where T : class
        {
            if (string.IsNullOrEmpty(jsonObj))
            {
                throw new ArgumentNullException(nameof(jsonObj));
            }
            var targetType = typeof(T);
            var childrenOfTargetType = AllAssemblyTypes.Where(t => t.IsSubclassOf(targetType)).ToList();
            foreach (var childType in childrenOfTargetType)
            {
                try
                {
                    var obj = JsonSerializer.Deserialize(jsonObj, childType);
                    if (obj is not null)
                    {
                        return (obj as T)!;
                    }
                }
                catch (Exception) { }
            }
            throw new InvalidCastException($"Cannot cast object");
        }
    }
}