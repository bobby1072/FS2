using System.Net;
using fsCore.Api.Attributes;
using fsCore.Common.Misc;
using fsCore.Common.Models;
using fsCore.Services.Abstract;
using fsCore.Services.Concrete;
using Microsoft.AspNetCore.SignalR;

namespace fsCore.Api.Hubs.Filters
{
    public class RequiredSignalRUserConnectionIdFilter : IHubFilter
    {
        private readonly ICachingService _cachingService;

        public RequiredSignalRUserConnectionIdFilter(ICachingService cachingService)
        {
            _cachingService = cachingService;
        }

        public async ValueTask<object?> InvokeMethodAsync(
            HubInvocationContext invocationContext,
            Func<HubInvocationContext, ValueTask<object?>> next
        )
        {
            var requiredConnectionCaching =
                invocationContext.GetMetadata<RequiredSignalRUserConnectionId>();
            if (requiredConnectionCaching is not null)
            {
                var connectionId =
                    invocationContext.Context.ConnectionId
                    ?? throw new ApiException(
                        ErrorConstants.NotAuthorized,
                        HttpStatusCode.Unauthorized
                    );
                var token =
                    invocationContext.Context.GetHttpContext()?.Request.Headers.Authorization
                    ?? throw new ApiException(
                        ErrorConstants.NotAuthorized,
                        HttpStatusCode.Unauthorized
                    );
                var existingUser =
                    await _cachingService.TryGetObject<User>($"{User.CacheKeyPrefix}{token}")
                    ?? throw new ApiException(
                        ErrorConstants.NotAuthorized,
                        HttpStatusCode.Unauthorized
                    );
                var existingUserConnection = await _cachingService.TryGetObject<string>(
                    $"{RequiredSignalRUserConnectionId.ConnectionIdUserIdCacheKeyPrefix}{existingUser.Id}"
                );
                if (existingUserConnection is null)
                {
                    await _cachingService.SetObject(
                        $"{RequiredSignalRUserConnectionId.ConnectionIdUserIdCacheKeyPrefix}{existingUser.Id}",
                        connectionId,
                        CacheObjectTimeToLiveInSeconds.OneHour
                    );
                }
            }
            return await next.Invoke(invocationContext);
        }
    }
}
