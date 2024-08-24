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
                catch (ApiException apiException)
                {
                    LogError(apiException.Message, Context);
                }
                catch (ValidationException validationException)
                {
                    LogError(CreateValidationExceptionMessage(validationException) ?? ErrorConstants.BadRequest, Context);
                }
                catch (NpgsqlException)
                {
                    LogError(ErrorConstants.FailedToPersistData, Context);
                }
                catch (Exception)
                {
                    LogError(ErrorConstants.InternalServerError, Context);
                }
            }
            catch { }
        }
        private void LogError(string message, HubCallerContext context)
        {
            _logger.LogError("Signal R {Connection} failed with {Exception}", context.ConnectionId, message);
        }
        private static string CreateValidationExceptionMessage(ValidationException validationException)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < validationException.Errors.Count(); i++)
            {
                var error = validationException.Errors.ElementAt(i);
                sb.Append($"{error.ErrorMessage}. ");
            }
            return sb.ToString();
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