using Common.Misc;
using Common.Models;
using fsCore.Attributes;
using fsCore.Hubs.Filters.Abstract;
using Microsoft.AspNetCore.SignalR;
using Services.Abstract;
using System.Net;

namespace fsCore.Hubs.Filters.Concrete
{
    public class RequiredSignalRUserConnectionIdFilter : IRequiredSignalRUserConnectionIdFilter
    {
        private const string ConnectionIdUserIdCacheKeyPrefix = "connectionIdUserId-";
        private readonly ICachingService _cachingService;
        public RequiredSignalRUserConnectionIdFilter(ICachingService cachingService)
        {
            _cachingService = cachingService;
        }
        public async ValueTask<object?> InvokeMethodAsync(
                HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
        {
            var requiredConnectionCaching = invocationContext.GetMetadata<RequiredSignalRUserConnectionId>();
            if (requiredConnectionCaching is not null)
            {
                var connectionId = invocationContext.Context.ConnectionId ?? throw new ApiException(ErrorConstants.NotAuthorized, HttpStatusCode.Unauthorized);
                var token = invocationContext.Context.GetHttpContext()?.Request.Headers.Authorization ?? throw new ApiException(ErrorConstants.NotAuthorized, HttpStatusCode.Unauthorized);
                var existingUser = await _cachingService.TryGetObject<User>($"{User.CacheKeyPrefix}{token}") ?? throw new ApiException(ErrorConstants.NotAuthorized, HttpStatusCode.Unauthorized);
                var existingUserConnection = await _cachingService.TryGetObject<string>($"{ConnectionIdUserIdCacheKeyPrefix}{existingUser.Id}");
                if (existingUserConnection is null)
                {
                    await _cachingService.SetObject($"{ConnectionIdUserIdCacheKeyPrefix}{existingUser.Id}", connectionId);
                }
            }
            return await next.Invoke(invocationContext);
        }
    }
}