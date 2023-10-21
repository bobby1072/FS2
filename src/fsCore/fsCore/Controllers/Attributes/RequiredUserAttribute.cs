using System.Net;
using System.Text.Json;
using Common;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace fsCore.Controllers.Attributes
{
    internal sealed class RequiredUser : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context is AuthorizationFilterContext foundContext)
            {
                if (foundContext.HttpContext.User.Identity?.IsAuthenticated == true)
                {
                    var user = foundContext.HttpContext.Session.GetString("user");
                    if (user is null)
                    {
                        foundContext.Result = new UnauthorizedResult();
                        return;
                    }
                    var _ = JsonSerializer.Deserialize<User>(user) ?? throw new ApiException(ErrorConstants.InternalServerError, HttpStatusCode.InternalServerError);
                    return;
                }
                foundContext.Result = new UnauthorizedResult();
                return;
            }
        }
    }
}