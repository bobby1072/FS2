using Common;
using Common.Models;
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
        public BaseController(ILogger logger)
        {
            _logger = logger;
        }
        protected User _getCurrentUser()
        {
            var user = HttpContext.Session.GetString("user") ?? throw new ApiException(ErrorConstants.NotAuthorised, HttpStatusCode.Unauthorized);
            var parsedUser = JsonSerializer.Deserialize<User>(user) ?? throw new ApiException(ErrorConstants.InternalServerError, HttpStatusCode.InternalServerError);
            return parsedUser;
        }
    }
}
