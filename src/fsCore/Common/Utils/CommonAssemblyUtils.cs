using System.Reflection;
using Common.Attributes;

namespace Common.Utils
{
    public static class CommonAssemblyUtils
    {
        public static readonly Assembly RuntimeAssembly = Assembly.GetExecutingAssembly();
        public static readonly IReadOnlyCollection<Type> AllAssemblyTypes = RuntimeAssembly.GetTypes();
        /// <summary>
        /// <para>Parse object to child of T</para>
        /// <para>Requires child classes you want to parse to to have an AssemblyConstructor</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ParseToChildOf<T>(object? obj) where T : class
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            var targetType = typeof(T);
            var childClasses = AllAssemblyTypes.Where(x => x.IsSubclassOf(targetType)).ToArray();
            foreach (var childType in childClasses)
            {
                try
                {

                    var objConstructor = Array.Find(childType.GetConstructors(), x => x.GetCustomAttribute<AssemblyConstructorAttribute>() is not null) ?? throw new InvalidDataException("No assembly constructor found");
                    var parsedObj = objConstructor.Invoke(new object[] { obj });
                    if (parsedObj is not null)
                    {
                        return (parsedObj as T)!;
                    }
                }
                //can be ignored as need to try constructors til you get a working one
                catch { }
            }
            throw new InvalidCastException($"Cannot cast object");
        }
    }
}