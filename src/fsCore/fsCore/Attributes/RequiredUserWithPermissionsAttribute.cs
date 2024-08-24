using System.Net;
using System.Text.Json;
using Common;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace fsCore.Attributes
{
    internal sealed class RequiredUserWithGroupPermissions : AuthorizeAttribute, IAuthorizationFilter
    {
        public bool UpdateAlways { get; set; }
        public RequiredUserWithGroupPermissions(bool updateAfter = false)
        {
            UpdateAlways = updateAfter;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {

            if (context.HttpContext.User.Identity?.IsAuthenticated == true)
            {
                if (context.HttpContext.Request.Headers.Authorization.FirstOrDefault() is null)
                {
                    throw new ApiException(ErrorConstants.NotAuthorized, HttpStatusCode.Unauthorized);
                }
                return;
            }
            context.Result = new UnauthorizedResult();
            return;
        }
    }
}