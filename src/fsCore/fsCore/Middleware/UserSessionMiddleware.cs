using System.Net;
using System.Text.Json;
using Common;
using Common.Models;
using Common.Utils;
using fsCore.Controllers.Attributes;
using fsCore.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace fsCore.Middleware
{
    internal class UserSessionMiddleware : BaseMiddleware
    {
        public UserSessionMiddleware(RequestDelegate next) : base(next) { }
        public async Task InvokeAsync(HttpContext httpContext, IUserService userService)
        {
            var endpoint = httpContext.GetEndpoint();
            if (endpoint?.Metadata.GetMetadata<RequiredUser>() is not null)
            {
                var tokenUser = httpContext
                    .GetTokenData()?
                    .TokenClaimsToUser() ??
                    throw new ApiException(ErrorConstants.NotAuthorized, HttpStatusCode.Unauthorized);
                var existingUserSession = httpContext.Session.GetString("user");
                if (existingUserSession is null)
                {
                    var userExistence = await userService.CheckUserExistsAndCreateIfNot(tokenUser);
                    httpContext.Session.SetString("user", JsonSerializer.Serialize(userExistence));
                }
                else if (JsonSerializer.Deserialize<User>(existingUserSession) is not User user)
                {
                    throw new ApiException(ErrorConstants.InternalServerError, HttpStatusCode.InternalServerError);
                }
                else if (user.Name != tokenUser.Name)
                {
                    var userExistence = await userService.UpdateUser(tokenUser);
                    httpContext.Session.SetString("user", JsonSerializer.Serialize(userExistence));
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