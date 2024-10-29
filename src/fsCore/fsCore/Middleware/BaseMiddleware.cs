using Common.Models;
using Services.Abstract;

namespace fsCore.Middleware
{
    internal abstract class BaseMiddleware
    {
        protected readonly RequestDelegate _next;
        public BaseMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        protected static async Task<User?> GetUserFromCache(ICachingService cachingService, HttpContext reqContext)
        {
            return await cachingService.TryGetObject<User>($"{User.CacheKeyPrefix}{GetTokenString(reqContext)}");
        }
        protected static async Task<UserWithGroupPermissionSet?> GetUserWithPermissionsFromCache(ICachingService cachingService, HttpContext reqContext)
        {
            return await cachingService.TryGetObject<UserWithGroupPermissionSet>($"{UserWithGroupPermissionSet.CacheKeyPrefix}{GetTokenString(reqContext)}");
        }
        private static string GetTokenString(HttpContext reqContext)
        {
            return reqContext.Request.Headers.Authorization.FirstOrDefault() ?? throw new InvalidDataException("Can't find auth token");
        }
    }
}