using System.Net;
using System.Text.Json;
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
                var token = httpContext.Request.Headers.Authorization.First() ?? throw new ApiException(ErrorConstants.NotAuthorized, HttpStatusCode.Unauthorized);
                var existingUserSession = httpContext.Session.GetString(RuntimeConstants.UserSession);
                if (existingUserSession is null)
                {
                    var tokenUser = await userInfoClient.GetUserInfoReturnUser(token);
                    var userExistence = await userService.CheckUserExistsAndCreateIfNot(tokenUser);
                    await cacheService.SetObject(userExistence.Id?.ToString() ?? throw new InvalidDataException("Cannot find user id"), userExistence);
                }
                else if (existingUserSession is not null && foundAttribute.UpdateAlways)
                {
                    var userFound = await userService.GetUser(JsonSerializer.Deserialize<User>(existingUserSession)?.Id ?? throw new InvalidDataException());
                    await cacheService.SetObject(userFound.Id?.ToString() ?? throw new InvalidDataException("Cannot find user id"), userFound);
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