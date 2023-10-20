using System.Net;
using System.Text.Json;
using Common;
using Common.Models;
using Common.Utils;
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
            if (endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>() is not null || endpoint?.Metadata.GetMetadata<AuthorizeAttribute>() is null)
            {
                await _next(httpContext);
                return;
            }
            if (httpContext.User.Identity?.IsAuthenticated is false)
            {
                throw new ApiException(ErrorConstants.NotAuthorized, HttpStatusCode.Unauthorized);
            }
            var tokenUser = httpContext
                .GetTokenData()?
                .TokenClaimsToUser() ??
                throw new ApiException(ErrorConstants.NotAuthorized, HttpStatusCode.Unauthorized);
            var existingUserSession = httpContext.Session.GetString("user");
            if (string.IsNullOrEmpty(existingUserSession))
            {
                var userExistence = await userService.CheckUserExistsAndCreateIfNot(tokenUser);
                httpContext.Session.SetString("user", JsonSerializer.Serialize(userExistence));
            }
            else if (JsonSerializer.Deserialize<User>(existingUserSession) is not User user)
            {
                throw new ApiException(ErrorConstants.InternalServerError, HttpStatusCode.InternalServerError);
            }
            else if (user.Email != tokenUser.Email)
            {
                var userExistence = await userService.CheckUserExistsAndCreateIfNot(tokenUser);
                httpContext.Session.SetString("user", JsonSerializer.Serialize(userExistence));
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