using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Common;
using Common.Utils;
using fsCore.Service;
using Microsoft.AspNetCore.Authorization;

namespace fsCore.Middleware
{
    internal class UserSessionMiddleware
    {
        private readonly RequestDelegate _next;
        public UserSessionMiddleware(RequestDelegate next)
        {
            _next = next;

        }
        public async Task InvokeAsync(HttpContext httpContext, IUserService userService)
        {
            var endpoint = httpContext.GetEndpoint();
            if (endpoint?.Metadata.GetMetadata<AuthorizeAttribute>() is null)
            {
                await _next(httpContext);
            }
            if (httpContext.User.Identity?.IsAuthenticated is false)
            {
                throw new ApiException(ErrorConstants.NotAuthorised, HttpStatusCode.Unauthorized);
            }
            var tokenData = httpContext.Request.Headers.Authorization
                .First()?
                .GetTokenData()?
                .TokenClaimsToUser() ??
                throw new ApiException(ErrorConstants.NotAuthorised, HttpStatusCode.Unauthorized);
            var existingUserSession = httpContext.Session.GetString("email");
            if (existingUserSession is null || existingUserSession != tokenData.Email)
            {
                var userExistence = await userService.CheckUserExistsAndCreateIfNot(tokenData);
                httpContext.Session.SetString("email", userExistence.Email);
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