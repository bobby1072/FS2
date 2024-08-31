using System.Net;
using Common;
using Common.Models;
using Microsoft.AspNetCore.SignalR;
using Services.Abstract;

namespace fsCore.Hubs
{
    public abstract class BaseHub : Hub
    {
        protected readonly ICachingService _cachingService;
        protected BaseHub(ICachingService cachingService)
        {
            _cachingService = cachingService;
        }
        protected HttpContext? HttpContext
        {
            get
            {
                HttpContext ??= Context.GetHttpContext();
                return HttpContext;
            }
            set { }
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
    }
}