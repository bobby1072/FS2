using Common.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace Common.Utils
{
    public static class JwtExtensions
    {
        public static User TokenClaimsToUser(this JwtSecurityToken token)
        {
            var email = token.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? throw new ApiException(ErrorConstants.NotAuthorised, HttpStatusCode.Unauthorized);
            var emailVerified = token.Claims.FirstOrDefault(c => c.Type == "emailVerified")?.Value ?? throw new ApiException(ErrorConstants.NotAuthorised, HttpStatusCode.Unauthorized);
            var name = token.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            return new User(email, Convert.ToBoolean(emailVerified), name);
        }
    }
}