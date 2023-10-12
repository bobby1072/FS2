using Common;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

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
        protected async Task<IActionResult> _routeErrorHandler<T>(T error) where T : Exception
        {
            if (error is ApiException apiException)
            {
                return StatusCode((int)apiException.StatusCode, apiException.Message);
            }
            return StatusCode((int)HttpStatusCode.InternalServerError, string.IsNullOrEmpty(error.Message) ? ErrorConstants.InternalServerError : error.Message);
        }
    }
}
