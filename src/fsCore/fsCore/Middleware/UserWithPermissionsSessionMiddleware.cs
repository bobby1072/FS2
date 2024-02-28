using System.Net;
using System.Text.Json;
using Common;
using Common.Models;
using fsCore.Controllers.Attributes;
using fsCore.Service.Interfaces;

namespace fsCore.Middleware
{
    internal class UserWithPermissionsSessionMiddleware : BaseMiddleware
    {
        public UserWithPermissionsSessionMiddleware(RequestDelegate next) : base(next) { }
        public async Task InvokeAsync(HttpContext httpContext, IGroupService groupService)
        {
            var endpointData = httpContext.GetEndpoint();
            if (endpointData?.Metadata.GetMetadata<RequiredUserWithPermissions>() is RequiredUserWithPermissions foundAttribute)
            {
                var user = httpContext.Session.GetString("user") ?? throw new ApiException(ErrorConstants.NotAuthorized, HttpStatusCode.Unauthorized);
                var parsedUser = JsonSerializer.Deserialize<User>(user) ?? throw new ApiException(ErrorConstants.InternalServerError, HttpStatusCode.InternalServerError);
                var foundUserWithPermissions = httpContext.Session.GetString("userWithPermissions");
                if (!foundAttribute.UpdateAfter && foundUserWithPermissions is not null)
                {
                    await _next(httpContext);
                    return;
                }
                else if (foundUserWithPermissions is null)
                {
                    var (groups, members) = await groupService.GetAllGroupsAndMembershipsForUser(parsedUser);
                    var newUserWithPermissions = new UserWithGroupPermissionSet(parsedUser);
                    newUserWithPermissions.BuildPermissions(groups);
                    newUserWithPermissions.BuildPermissions(members);
                    httpContext.Session.SetString("userWithPermissions", JsonSerializer.Serialize(newUserWithPermissions));
                }
                if (foundAttribute.UpdateAfter)
                {
                    await _next(httpContext);
                    var (groups, members) = await groupService.GetAllGroupsAndMembershipsForUser(parsedUser);
                    var newUserWithPermissions = new UserWithGroupPermissionSet(parsedUser);
                    newUserWithPermissions.BuildPermissions(groups);
                    newUserWithPermissions.BuildPermissions(members);
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