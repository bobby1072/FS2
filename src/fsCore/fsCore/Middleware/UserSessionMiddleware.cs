using System.Net;
using System.Text.Json;
using Common;
using Common.Models;
using fsCore.Service;
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
                var existingUserSession = httpContext.Session.GetString(RuntimeConstants.UserSession);
                if (existingUserSession is null)
                {
                    var tokenUser = await userInfoClient.GetUserInfoReturnUser(token);
                    var userExistence = await userService.CheckUserExistsAndCreateIfNot(tokenUser);
                    httpContext.Session.SetString(RuntimeConstants.UserSession, JsonSerializer.Serialize(userExistence));
                }
                else if (existingUserSession is not null && foundAttribute.UpdateAlways)
                {
                    var userFound = await userService.GetUser(JsonSerializer.Deserialize<User>(existingUserSession)?.Id ?? throw new Exception());
                    httpContext.Session.SetString(RuntimeConstants.UserSession, JsonSerializer.Serialize(userFound));
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