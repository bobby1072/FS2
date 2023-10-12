using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Common;
using Common.Models;
using fsCore.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fsCore.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService, ILogger<UserController> logger) : base(logger)
        {
            _userService = userService;
        }
        private static User _tokenClaimsToUser(JwtSecurityToken token)
        {
            var email = token.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? throw new ApiException(ErrorConstants.NotAuthorised, HttpStatusCode.Unauthorized);
            var emailVerified = token.Claims.FirstOrDefault(c => c.Type == "emailVerified")?.Value ?? throw new ApiException(ErrorConstants.NotAuthorised, HttpStatusCode.Unauthorized);
            var name = token.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            return new User(email, Convert.ToBoolean(emailVerified), name);
        }
        [ProducesDefaultResponseType(typeof(string))]
        [HttpGet("StartSession")]
        public async Task<IActionResult> StartUserSession()
        {
            try
            {
                var tokenData = _getTokenData() ?? throw new ApiException(ErrorConstants.NotAuthorised, HttpStatusCode.Unauthorized);
                var userExistence = await _userService.CheckUserExistsAndCreateIfNot(_tokenClaimsToUser(tokenData));
                ControllerContext.HttpContext.Session.SetString("email", userExistence.Email);
                return Ok("Session started");
            }
            catch (Exception e)
            {
                return await _routeErrorHandler(e);
            }
        }
    }
}