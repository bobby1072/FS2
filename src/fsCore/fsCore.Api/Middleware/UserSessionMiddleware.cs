using System.Net;
using fsCore.Api.Attributes;
using fsCore.Common.Misc;
using fsCore.Common.Models;
using fsCore.Services.Abstract;
using fsCore.Services.Concrete;

namespace fsCore.Api.Middleware
{
    internal class UserSessionMiddleware : BaseMiddleware
    {
        public UserSessionMiddleware(RequestDelegate next)
            : base(next) { }

        public async Task InvokeAsync(
            HttpContext httpContext,
            IUserService userService,
            IUserInfoClient userInfoClient,
            ICachingService cacheService
        )
        {
            var endpoint = httpContext.GetEndpoint();
            if (endpoint?.Metadata.GetMetadata<RequiredUser>() is RequiredUser foundAttribute)
            {
                var token =
                    httpContext.Request.Headers.Authorization.FirstOrDefault()
                    ?? throw new ApiException(
                        ErrorConstants.NotAuthorized,
                        HttpStatusCode.Unauthorized
                    );
                var existingUserSession = await GetUserFromCache(cacheService, httpContext);
                if (existingUserSession is null)
                {
                    var tokenUser = await userInfoClient.GetUserInfoReturnUser(token);
                    var userExistence = await userService.CheckUserExistsAndCreateIfNot(tokenUser);
                    await cacheService.SetObject(
                        $"{User.CacheKeyPrefix}{token}",
                        userExistence,
                        CacheObjectTimeToLiveInSeconds.OneHour
                    );
                }
                else if (existingUserSession is not null && foundAttribute.UpdateAlways)
                {
                    var userFound = await userService.GetUser((Guid)existingUserSession.Id!);
                    await cacheService.SetObject(
                        $"{User.CacheKeyPrefix}{token}",
                        userFound,
                        CacheObjectTimeToLiveInSeconds.OneHour
                    );
                }
            }
            await _next.Invoke(httpContext);
        }
    }

    internal static class UserSessionMiddlewareExtensions
    {
        public static IApplicationBuilder UseUserSessionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserSessionMiddleware>();
        }
    }
}
