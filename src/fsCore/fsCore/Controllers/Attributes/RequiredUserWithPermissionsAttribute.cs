using System.Net;
using System.Text.Json;
using Common;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace fsCore.Controllers.Attributes
{
    internal sealed class RequiredUserWithPermissions : AuthorizeAttribute, IAuthorizationFilter
    {
        public bool UpdateAlways { get; set; }
        public RequiredUserWithPermissions(bool updateAfter = false)
        {
            UpdateAlways = updateAfter;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {

            if (context.HttpContext.User.Identity?.IsAuthenticated == true)
            {
                var user = context.HttpContext.Session.GetString(RuntimeConstants.UserWithPermissionsSession);
                if (user is null)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }
                var _ = JsonSerializer.Deserialize<UserWithGroupPermissionSet>(user) ?? throw new ApiException(ErrorConstants.InternalServerError, HttpStatusCode.InternalServerError);
                return;
            }
            context.Result = new UnauthorizedResult();
            return;
        }
    }
}