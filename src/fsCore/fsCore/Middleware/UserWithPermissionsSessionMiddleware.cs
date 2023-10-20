using System.Net;
using System.Text.Json;
using Common;
using Common.Models;
using fsCore.Service.Interfaces;

namespace fsCore.Middleware
{
    internal class UserWithPermissionsSessionMiddleware : BaseMiddleware
    {
        public UserWithPermissionsSessionMiddleware(RequestDelegate next) : base(next) { }
        public async Task InvokeAsync(HttpContext httpContext, IGroupService groupService)
        {
            var routeData = httpContext.GetRouteData();
            var controllerName = routeData.Values["controller"]?.ToString()?.ToLower();
            var action = routeData.Values["action"]?.ToString()?.ToLower();
            var user = httpContext.Session.GetString("user");
            if (user is not null)
            {
                var parsedUser = JsonSerializer.Deserialize<User>(user) ?? throw new ApiException(ErrorConstants.InternalServerError, HttpStatusCode.InternalServerError);
                var foundUserWithPermissions = httpContext.Session.GetString("userWithPermissions");
                if (foundUserWithPermissions is not null && JsonSerializer.Deserialize<UserWithGroupPermissionSet>(foundUserWithPermissions) is null)
                {
                    throw new ApiException(ErrorConstants.InternalServerError, HttpStatusCode.InternalServerError);
                }
                if ((action?.Contains("create") == true || action?.Contains("update") == true || action?.Contains("create") == true) == false && foundUserWithPermissions is not null)
                {
                    await _next(httpContext);
                    return;
                }
                else if (foundUserWithPermissions is null)
                {
                    var foundGroupMembers = await groupService.TryGetAllMembersForUserIncludingGroupAndUserAndPosition(parsedUser);
                    if (foundGroupMembers is null)
                    {
                        await _next(httpContext);
                        return;
                    }
                    var newUserWithPermissions = new UserWithGroupPermissionSet(parsedUser.Email, parsedUser.EmailVerified, parsedUser.Name, foundGroupMembers);
                    httpContext.Session.SetString("userWithPermissions", JsonSerializer.Serialize(newUserWithPermissions));
                }
                if (action == "create" || action == "update" || action == "delete")
                {
                    await _next(httpContext);
                    var foundGroupMembers = await groupService.TryGetAllMembersForUserIncludingGroupAndUserAndPosition(parsedUser);
                    if (foundGroupMembers is null)
                    {
                        return;
                    }
                    var newUserWithPermissions = new UserWithGroupPermissionSet(parsedUser.Email, parsedUser.EmailVerified, parsedUser.Name, foundGroupMembers);
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