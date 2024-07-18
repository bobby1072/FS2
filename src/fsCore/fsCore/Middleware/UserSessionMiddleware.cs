using System.Net;
using Common;
using Common.Models;
using fsCore.Controllers.Attributes;
using fsCore.Services.Abstract;
using Services.Abstract;

namespace fsCore.Middleware
{
    internal class UserSessionMiddleware : BaseMiddleware
    {
        public UserSessionMiddleware(RequestDelegate next) : base(next) { }
        public async Task InvokeAsync(HttpContext httpContext, IUserService userService, IUserInfoClient userInfoClient, ICachingService cacheService)
        {
            var endpoint = httpContext.GetEndpoint();
            if (endpoint?.Metadata.GetMetadata<RequiredUser>() is RequiredUser foundAttribute)
            {
                var token = httpContext.Request.Headers.Authorization.FirstOrDefault() ?? throw new ApiException(ErrorConstants.NotAuthorized, HttpStatusCode.Unauthorized);
                var existingUserSession = await GetUserFromCache(cacheService, httpContext);
                if (existingUserSession is null)
                {
                    var tokenUser = await userInfoClient.GetUserInfoReturnUser(token);
                    var userExistence = await userService.CheckUserExistsAndCreateIfNot(tokenUser);
                    await cacheService.SetObject($"{User.CacheKeyPrefix}{token}", userExistence);
                }
                else if (existingUserSession is not null && foundAttribute.UpdateAlways)
                {
                    var userFound = await userService.GetUser(existingUserSession?.Id ?? throw new InvalidDataException("Cannot deserialize user"));
                    await cacheService.SetObject($"{User.CacheKeyPrefix}{token}", userFound);
                }
            }
            await _next(httpContext);

        }
    }
    internal static class UserSessionMiddlewareExtensions
    {
        public static IApplicationBuilder UseUserSessionMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserSessionMiddleware>();
        }
    }
}