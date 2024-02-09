using System.Net;
using System.Text.Json;
using Common;
using Common.Models;
using fsCore.Contexts;
using fsCore.Controllers.Attributes;
using fsCore.Service.Interfaces;

namespace fsCore.Middleware
{
    internal class UserSessionMiddleware : BaseMiddleware
    {
        public UserSessionMiddleware(RequestDelegate next) : base(next) { }
        public async Task InvokeAsync(HttpContext httpContext, IUserService userService, IUserInfoClient userInfoClient)
        {
            var endpoint = httpContext.GetEndpoint();
            if (endpoint?.Metadata.GetMetadata<RequiredUser>() is RequiredUser foundAttribute)
            {
                var token = httpContext.Request.Headers.Authorization.First() ?? throw new ApiException(ErrorConstants.NotAuthorized, HttpStatusCode.Unauthorized);
                var existingUserSession = httpContext.Session.GetString("user");
                if (existingUserSession is null)
                {
                    var tokenUser = await userInfoClient.GetUserInfoReturnUser(token);
                    var userExistence = await userService.CheckUserExistsAndCreateIfNot(tokenUser);
                    httpContext.Session.SetString("user", JsonSerializer.Serialize(userExistence));
                    await _next(httpContext);
                    if (foundAttribute.UpdateAfter)
                    {
                        var foundUser = await userService.GetUser(userExistence.Email);
                        httpContext.Session.SetString("user", JsonSerializer.Serialize(foundUser));
                    }
                    return;
                }
                if (foundAttribute.UpdateAfter)
                {
                    var tokenUser = await userInfoClient.GetUserInfoReturnUser(token);
                    var userExistence = await userService.CheckUserExistsAndCreateIfNot(tokenUser);
                    httpContext.Session.SetString("user", JsonSerializer.Serialize(userExistence));
                }
                await _next(httpContext);
                return;
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