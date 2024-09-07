using Common.Misc;
using Common.Models;
using Common.Utils;
using Microsoft.AspNetCore.Mvc;
using Services.Abstract;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace fsCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected readonly ILogger _logger;
        protected ICachingService _cachingService;
        protected BaseController(ILogger logger, ICachingService cachingService)
        {
            _logger = logger;
            _cachingService = cachingService;
        }
        protected JwtSecurityToken? GetTokenData()
        {
            return ControllerContext.HttpContext.GetTokenData();
        }
        protected async Task<User> GetCurrentUserAsync()
        {
            return await _cachingService.TryGetObject<User>($"{Common.Models.User.CacheKeyPrefix}{GetTokenString()}") ?? throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Unauthorized);

        }
        protected async Task<UserWithGroupPermissionSet> GetCurrentUserWithPermissionsAsync()
        {
            return await _cachingService.TryGetObject<UserWithGroupPermissionSet>($"{UserWithGroupPermissionSet.CacheKeyPrefix}{GetTokenString()}") ?? throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Unauthorized);
        }
        private string GetTokenString()
        {
            return ControllerContext.HttpContext.Request.Headers.Authorization.FirstOrDefault() ?? throw new InvalidDataException("Can't find auth token");
        }
    }
}
