using Microsoft.AspNetCore.SignalR;
using System.Reflection;

namespace fsCore.Hubs.Filters.Concrete
{
    public static class FilterContextExtensions
    {
        public static T? GetMetadata<T>(this HubInvocationContext invocationContext) where T : Attribute
        {
            return invocationContext.HubMethod.GetCustomAttribute<T>() ?? invocationContext.Hub.GetType().GetCustomAttribute<T>();
        }
    }
}