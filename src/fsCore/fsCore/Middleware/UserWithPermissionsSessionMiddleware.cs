using System.Net;
using System.Text.Json;
using Common;
using Common.Models;
using fsCore.Controllers.Attributes;
using fsCore.Service.Abstract;

namespace fsCore.Middleware
{
    internal class UserWithPermissionsSessionMiddleware : BaseMiddleware
    {
        public UserWithPermissionsSessionMiddleware(RequestDelegate next) : base(next) { }
        public async Task InvokeAsync(HttpContext httpContext, IGroupService groupService)
        {
            var endpointData = httpContext.GetEndpoint();
            if (endpointData?.Metadata.GetMetadata<RequiredUserWithGroupPermissions>() is RequiredUserWithGroupPermissions foundAttribute)
            {
                var foundUserWithPermissions = httpContext.Session.GetString(RuntimeConstants.UserWithPermissionsSession);
                if (foundAttribute.UpdateAlways || foundUserWithPermissions is null)
                {
                    var user = httpContext.Session.GetString(RuntimeConstants.UserSession) ?? throw new ApiException(ErrorConstants.NotAuthorized, HttpStatusCode.Unauthorized);
                    var parsedUser = JsonSerializer.Deserialize<User>(user) ?? throw new ApiException(ErrorConstants.InternalServerError, HttpStatusCode.InternalServerError);
                    var (groups, members) = await groupService.GetAllGroupsAndMembershipsForUser(parsedUser);
                    var newUserWithPermissions = new UserWithGroupPermissionSet(parsedUser);
                    newUserWithPermissions.BuildPermissions(groups);
                    newUserWithPermissions.BuildPermissions(members);
                    httpContext.Session.SetString(RuntimeConstants.UserWithPermissionsSession, JsonSerializer.Serialize(newUserWithPermissions));
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