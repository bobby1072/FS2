using Common;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace fsCore.Controllers
{
    public class BaseController : ControllerBase
    {

        protected async Task<IActionResult> _routeErrorHandler<T>(T error) where T : Exception
        {
            if (error is ApiException apiException)
            {
                return StatusCode(apiException.StatusCode.HasValue ? (int)apiException.StatusCode : (int)HttpStatusCode.InternalServerError, string.IsNullOrEmpty(apiException.Message) ? ErrorConstants.InternalServerError : apiException.Message);
            }
            return StatusCode((int)HttpStatusCode.InternalServerError, string.IsNullOrEmpty(error.Message) ? ErrorConstants.InternalServerError : error.Message);
        }
        protected JwtSecurityToken? _getTokenData()
        {
            try
            {
                var bearer = ControllerContext.HttpContext.Request.Headers.Authorization.First();
                if (string.IsNullOrEmpty(bearer))
                {
                    throw new Exception();
                }
                var handler = new JwtSecurityTokenHandler();
                var token = bearer.Split(" ").Last();
                var jsonToken = handler.ReadToken(token);
                return jsonToken as JwtSecurityToken;
            }
            catch (Exception _)
            {
                return null;
            }
        }
    }
}
