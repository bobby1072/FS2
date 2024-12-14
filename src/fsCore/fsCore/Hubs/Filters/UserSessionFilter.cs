using fsCore.Common.Misc;
using fsCore.Common.Models;
using fsCore.Attributes;
using Microsoft.AspNetCore.SignalR;
using Services.Abstract;
using Services.Concrete;
using System.Net;

namespace fsCore.Hubs.Filters
{
    public class UserSessionFilter : IHubFilter
    {
        private readonly IUserService _userService;
        private readonly IUserInfoClient _userInfoClient;
        private readonly ICachingService _cacheService;
        public UserSessionFilter(IUserService userService, IUserInfoClient userInfoClient, ICachingService cacheService)
        {
            _userService = userService;
            _userInfoClient = userInfoClient;
            _cacheService = cacheService;
        }
        public async ValueTask<object?> InvokeMethodAsync(
                HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
        {
            var hubRequireAttribute = invocationContext.GetMetadata<RequiredUser>();
            if (hubRequireAttribute is not null)
            {
                var token = invocationContext.Context.GetHttpContext()?.Request.Headers.Authorization ?? throw new ApiException(ErrorConstants.NotAuthorized, HttpStatusCode.Unauthorized);
                var existingUserSession = await _cacheService.TryGetObject<User>($"{User.CacheKeyPrefix}{token}");
                if (existingUserSession is null)
                {
                    var tokenUser = await _userInfoClient.GetUserInfoReturnUser(token!);
                    var userExistence = await _userService.CheckUserExistsAndCreateIfNot(tokenUser);
                    await _cacheService.SetObject($"{User.CacheKeyPrefix}{token}", userExistence, CacheObjectTimeToLiveInSeconds.OneHour);
                }
                else if (hubRequireAttribute.UpdateAlways)
                {
                    var userFound = await _userService.GetUser((Guid)existingUserSession.Id!);
                    await _cacheService.SetObject($"{User.CacheKeyPrefix}{token}", userFound, CacheObjectTimeToLiveInSeconds.OneHour);
                }
            }
            return await next.Invoke(invocationContext);
        }

    }
}