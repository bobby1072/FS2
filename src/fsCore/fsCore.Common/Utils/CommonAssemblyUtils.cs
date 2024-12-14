using fsCore.Common.Attributes;
using System.Reflection;

namespace fsCore.Common.Utils
{
    public static class CommonAssemblyUtils
    {
        private static readonly Assembly RuntimeAssembly = Assembly.GetExecutingAssembly();
        private static readonly IReadOnlyCollection<Type> AllAssemblyTypes = RuntimeAssembly.GetTypes();
        /// <summary>
        /// <para>Parse object to child of T</para>
        /// <para>Requires child classes you want to parse to to have an AssemblyConstructor</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ParseToChildOf<T>(object? obj) where T : class
        {
            ArgumentNullException.ThrowIfNull(obj);
            var targetType = typeof(T);
            var childClasses = AllAssemblyTypes.Where(x => x.IsSubclassOf(targetType)).ToArray();
            foreach (var childType in childClasses)
            {
                try
                {
                    var objConstructor = Array.Find(childType.GetConstructors(), x => x.GetCustomAttribute<AssemblyConstructorAttribute>() is not null) ?? throw new InvalidDataException("No assembly constructor found");
                    var parsedObj = objConstructor.Invoke([obj]);
                    if (parsedObj is not null)
                    {
                        return (parsedObj as T)!;
                    }
                }
                //can be ignored as need to try constructors til you get a working one
                catch { }
            }
            throw new InvalidCastException($"Cannot cast instance to type {typeof(T).Name}");
        }
    }
}