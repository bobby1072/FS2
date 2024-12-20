using System.Net;
using fsCore.Api.Attributes;
using fsCore.Common.Misc;
using fsCore.Common.Models;
using fsCore.Services.Abstract;
using fsCore.Services.Concrete;

namespace fsCore.Api.Middleware
{
    internal class UserWithPermissionsSessionMiddleware : BaseMiddleware
    {
        public UserWithPermissionsSessionMiddleware(RequestDelegate next)
            : base(next) { }

        public async Task InvokeAsync(
            HttpContext httpContext,
            IGroupService groupService,
            ICachingService cachingService
        )
        {
            var endpointData = httpContext.GetEndpoint();
            if (
                endpointData?.Metadata.GetMetadata<RequiredUserWithGroupPermissions>()
                is RequiredUserWithGroupPermissions foundAttribute
            )
            {
                var foundUserWithPermissions = GetUserWithPermissionsFromCache(
                    cachingService,
                    httpContext
                );
                if (foundAttribute.UpdateAlways || foundUserWithPermissions is null)
                {
                    var parsedUser =
                        await GetUserFromCache(cachingService, httpContext)
                        ?? throw new InvalidOperationException("RequireUserAttribute needed");
                    var (groups, members) = await groupService.GetAllGroupsAndMembershipsForUser(
                        parsedUser
                    );
                    var newUserWithPermissions = new UserWithGroupPermissionSet(parsedUser);
                    newUserWithPermissions.BuildPermissions(groups);
                    newUserWithPermissions.BuildPermissions(members);
                    var token =
                        httpContext.Request.Headers.Authorization.FirstOrDefault()
                        ?? throw new ApiException(
                            ErrorConstants.NotAuthorized,
                            HttpStatusCode.Unauthorized
                        );
                    await cachingService.SetObject(
                        $"{UserWithGroupPermissionSet.CacheKeyPrefix}{token}",
                        newUserWithPermissions,
                        CacheObjectTimeToLiveInSeconds.OneHour
                    );
                }
            }
            await _next.Invoke(httpContext);
        }
    }

    internal static class UserWithPermissionsSessionMiddlewareExtensions
    {
        public static IApplicationBuilder UseUserWithPermissionsSessionMiddleware(
            this IApplicationBuilder builder
        )
        {
            return builder.UseMiddleware<UserSessionMiddleware>();
        }
    }
}
