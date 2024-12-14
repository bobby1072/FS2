using fsCore.Common.Misc;
using fsCore.Common.Models;
using fsCore.Attributes;
using Microsoft.AspNetCore.SignalR;
using Services.Abstract;
using System.Net;

namespace fsCore.Hubs
{
    public abstract class BaseHub : Hub
    {
        protected readonly ICachingService _cachingService;
        protected BaseHub(ICachingService cachingService)
        {
            _cachingService = cachingService;
        }
        private HttpContext? _httpContext;
        protected HttpContext? HttpContext
        {
            get
            {
                _httpContext ??= Context.GetHttpContext();
                return _httpContext;
            }
            set
            {
                _httpContext = value;
            }
        }
        private string GetTokenString()
        {
            return HttpContext?.Request.Headers.Authorization.FirstOrDefault() ?? throw new InvalidDataException("Can't find auth token");
        }
        protected async Task<User> GetCurrentUserAsync()
        {
            return await _cachingService.TryGetObject<User>($"{User.CacheKeyPrefix}{GetTokenString()}") ?? throw new LiveMatchException(ErrorConstants.DontHavePermission, HttpStatusCode.Unauthorized);

        }
        protected async Task<UserWithGroupPermissionSet> GetCurrentUserWithPermissionsAsync()
        {
            return await _cachingService.TryGetObject<UserWithGroupPermissionSet>($"{UserWithGroupPermissionSet.CacheKeyPrefix}{GetTokenString()}") ?? throw new LiveMatchException(ErrorConstants.DontHavePermission, HttpStatusCode.Unauthorized);
        }
        protected async Task<string?> GetAnyUserConnectionIdAsync(Guid userId)
        {
            return await _cachingService.TryGetObject<string>($"{RequiredSignalRUserConnectionId.ConnectionIdUserIdCacheKeyPrefix}{userId}");
        }
    }
}