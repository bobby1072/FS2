using Common.Misc;
using Common.Models;
using fsCore.Attributes;
using fsCore.Hubs.Filters.Abstract;
using Microsoft.AspNetCore.SignalR;
using Services.Abstract;
using Services.Concrete;
using System.Net;

namespace fsCore.Hubs.Filters.Concrete
{
    public class UserWithPermissionsSessionFilter : IUserWithPermissionsSessionFilter
    {
        private readonly ICachingService _cachingService;
        private readonly IGroupService _groupService;
        public UserWithPermissionsSessionFilter(ICachingService cachingService, IGroupService groupService)
        {
            _cachingService = cachingService;
            _groupService = groupService;
        }
        public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
        {
            var hubRequireAttribute = invocationContext.GetMetadata<RequiredUserWithGroupPermissions>();

            if (hubRequireAttribute is not null)
            {
                var token = invocationContext.Context.GetHttpContext()?.Request.Headers.Authorization ?? throw new ApiException(ErrorConstants.NotAuthorized, HttpStatusCode.Unauthorized);
                var existingUserWithPermissions = await _cachingService.TryGetObject<UserWithGroupPermissionSet>($"{UserWithGroupPermissionSet.CacheKeyPrefix}{token}");
                if (existingUserWithPermissions is null || hubRequireAttribute.UpdateAlways == true)
                {
                    var parsedUser = await _cachingService.TryGetObject<User>($"{User.CacheKeyPrefix}{token}") ?? throw new ApiException(ErrorConstants.NotAuthorized, HttpStatusCode.Unauthorized);
                    (ICollection<Group> groups, ICollection<GroupMember> members) = await _groupService.GetAllGroupsAndMembershipsForUser(parsedUser);
                    var newUserWithPermissions = new UserWithGroupPermissionSet(parsedUser);
                    newUserWithPermissions.BuildPermissions(groups);
                    newUserWithPermissions.BuildPermissions(members);
                    await _cachingService.SetObject($"{UserWithGroupPermissionSet.CacheKeyPrefix}{token}", newUserWithPermissions, CacheObjectTimeToLiveInSeconds.OneHour);
                }
            }
            return await next.Invoke(invocationContext);
        }
    }
}