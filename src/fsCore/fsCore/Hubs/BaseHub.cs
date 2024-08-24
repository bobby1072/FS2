using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using Common;
using Common.Models;
using Common.Utils;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using Npgsql;
using Services.Abstract;

namespace fsCore.Hubs
{
    public abstract class BaseHub : Hub
    {
        protected readonly ICachingService _cachingService;
        private readonly ILogger<BaseHub> _logger;
        protected BaseHub(ICachingService cachingService, ILogger<BaseHub> logger)
        {
            _cachingService = cachingService;
            _logger = logger;
        }
        private HttpContext? _httpContext;
        protected HttpContext? HttpContext
        {
            get
            {
                _httpContext ??= Context.GetHttpContext();
                return _httpContext;
            }
        }
        protected async Task HandleErrors(Func<Task> methodFunc)
        {
            try
            {
                try
                {
                    await methodFunc.Invoke();
                }
                catch (Exception e)
                {
                    LogError(e, Context);
                }
            }
            catch { }
        }
        private void LogError(Exception exception, HubCallerContext context)
        {
            _logger.LogError("Signal R {Connection} failed with {Exception}", context.ConnectionId, exception);
        }
        protected JwtSecurityToken? GetTokenData()
        {
            return HttpContext?.GetTokenData();
        }
        private string GetTokenString()
        {
            return HttpContext?.Request.Headers.Authorization.FirstOrDefault() ?? throw new InvalidDataException("Can't find auth token");
        }
        protected async Task<User> GetCurrentUserAsync()
        {
            return await _cachingService.TryGetObject<User>($"{User.CacheKeyPrefix}{GetTokenString()}") ?? throw new LiveMatchException(ErrorConstants.DontHavePermission, HttpStatusCode.Unauthorized);

        }
        protected async Task<UserWithGroupPermissionSet> GetCurrentUserWithPermissionsAsync()
        {
            return await _cachingService.TryGetObject<UserWithGroupPermissionSet>($"{Common.Models.UserWithGroupPermissionSet.CacheKeyPrefix}{GetTokenString()}") ?? throw new LiveMatchException(ErrorConstants.DontHavePermission, HttpStatusCode.Unauthorized);
        }
    }
}