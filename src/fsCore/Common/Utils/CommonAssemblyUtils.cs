using System.Reflection;

namespace Common.Utils
{
    public static class CommonAssemblyUtils
    {
        public static readonly Assembly RuntimeAssembly = Assembly.GetExecutingAssembly();
        public static readonly IReadOnlyCollection<Type> AllAssemblyTypes = RuntimeAssembly.GetTypes();
    }
}