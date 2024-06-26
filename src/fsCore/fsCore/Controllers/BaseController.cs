﻿using Common;
using Common.Models;
using Common.Utils;
using Microsoft.AspNetCore.Mvc;
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

        protected JwtSecurityToken? GetTokenData()
        {
            return ControllerContext.HttpContext.GetTokenData();
        }
        public BaseController(ILogger logger)
        {
            _logger = logger;
        }
        protected User GetCurrentUser()
        {
            var user = HttpContext.Session.GetString(RuntimeConstants.UserSession) ?? throw new ApiException(ErrorConstants.NotAuthorized, HttpStatusCode.Unauthorized);
            var parsedUser = JsonSerializer.Deserialize<User>(user) ?? throw new ApiException(ErrorConstants.InternalServerError, HttpStatusCode.InternalServerError);
            return parsedUser;
        }
        protected UserWithGroupPermissionSet GetCurrentUserWithPermissions()
        {
            var user = HttpContext.Session.GetString(RuntimeConstants.UserWithPermissionsSession) ?? throw new ApiException(ErrorConstants.NotAuthorized, HttpStatusCode.Unauthorized);
            var parsedUser = JsonSerializer.Deserialize<UserWithGroupPermissionSet>(user) ?? throw new ApiException(ErrorConstants.InternalServerError, HttpStatusCode.InternalServerError);
            return parsedUser;

        }
    }
}
