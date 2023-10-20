using System.Net;
using System.Text.Json;
using Common;
using Common.Dbinterfaces.Repository;
using Common.Models;

namespace fsCore.Middleware
{
    internal class UserWithPermissionsSessionMiddleware : BaseMiddleware
    {
        public UserWithPermissionsSessionMiddleware(RequestDelegate next) : base(next) { }
        public async Task InvokeAsync(HttpContext httpContext, IGroupMemberRepository groupService)
        {
            var routeData = httpContext.GetRouteData();
            var controllerName = routeData.Values["controller"]?.ToString()?.ToLower();
            var action = routeData.Values["action"]?.ToString()?.ToLower();
            if (controllerName is not null && controllerName.ToLower() == "group")
            {
                var user = httpContext.Session.GetString("user") ?? throw new ApiException(ErrorConstants.NotAuthorized, HttpStatusCode.Unauthorized);
                var parsedUser = JsonSerializer.Deserialize<User>(user) ?? throw new ApiException(ErrorConstants.InternalServerError, HttpStatusCode.InternalServerError);
                var foundUserWithPermissions = httpContext.Session.GetString("userWithPermissions");
                if (foundUserWithPermissions is not null && JsonSerializer.Deserialize<UserWithGroupPermissionSet>(foundUserWithPermissions) is null)
                {
                    throw new ApiException(ErrorConstants.InternalServerError, HttpStatusCode.InternalServerError);
                }
                if (!(action == "create" || action == "update" || action == "delete") && foundUserWithPermissions is not null)
                {
                    await _next(httpContext);
                    return;
                }
                else if (foundUserWithPermissions is null)
                {
                    var foundGroupMembers = await groupService.GetManyGroupMemberForUserIncludingUserAndPositionAndGroup(parsedUser.Email);
                    if (foundGroupMembers is null)
                    {
                        await _next(httpContext);
                        return;
                    }
                    var newUserWithPermissions = new UserWithGroupPermissionSet(parsedUser.Email, parsedUser.EmailVerified, parsedUser.Name);
                    newUserWithPermissions.BuildPermissions(foundGroupMembers);
                    httpContext.Session.SetString("userWithPermissions", JsonSerializer.Serialize(newUserWithPermissions));
                    await _next(httpContext);
                    return;
                }
                else
                {
                    await _next(httpContext);
                    var foundGroupMembers = await groupService.GetManyGroupMemberForUserIncludingUserAndPositionAndGroup(parsedUser.Email);
                    if (foundGroupMembers is null)
                    {
                        return;
                    }
                    var newUserWithPermissions = new UserWithGroupPermissionSet(parsedUser.Email, parsedUser.EmailVerified, parsedUser.Name);
                    newUserWithPermissions.BuildPermissions(foundGroupMembers);
                    httpContext.Session.SetString("userWithPermissions", JsonSerializer.Serialize(newUserWithPermissions));
                    return;
                }
            }
            await _next(httpContext);
        }
    }
    internal static class UserWithPermissionsSessionMiddlewareExtensions
    {
        public static IApplicationBuilder UseUserWithPermissionsSessionMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserSessionMiddleware>();
        }
    }
}