using Common.Misc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace fsCore.Attributes
{
    internal sealed class RequiredUser : AuthorizeAttribute, IAuthorizationFilter
    {
        public bool UpdateAlways { get; set; }
        public RequiredUser(bool updateAfter = false)
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