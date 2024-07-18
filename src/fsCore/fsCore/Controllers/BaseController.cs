using Common;
using Common.Models;
using Common.Utils;
using Microsoft.AspNetCore.Mvc;
using Services.Abstract;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;

namespace fsCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected readonly ILogger _logger;
        protected ICachingService _cachingService;
        protected JwtSecurityToken? GetTokenData()
        {
            return ControllerContext.HttpContext.GetTokenData();
        }
        private string GetTokenString()
        {
            return ControllerContext.HttpContext.Request.Headers.Authorization.FirstOrDefault() ?? throw new InvalidDataException("Can't find auth token");
        }
        public BaseController(ILogger logger, ICachingService cachingService)
        {
            _logger = logger;
            _cachingService = cachingService;
        }
        protected async Task<User> GetCurrentUserAsync()
        {
            return await _cachingService.GetObject<User>($"{Common.Models.User.CacheKeyPrefix}{GetTokenString()}");

        }
        protected async Task<UserWithGroupPermissionSet> GetCurrentUserWithPermissionsAsync()
        {
            return await _cachingService.GetObject<UserWithGroupPermissionSet>($"{Common.Models.UserWithGroupPermissionSet.CacheKeyPrefix}{GetTokenString()}");
        }
    }
}
