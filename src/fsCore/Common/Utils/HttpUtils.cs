using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace Common.Utils
{
    public static class HttpUtils
    {
        public static JwtSecurityToken? GetTokenData(this HttpContext context)
        {
            try
            {
                var bearer = context.Request.Headers.Authorization.First();
                var handler = new JwtSecurityTokenHandler();
                var token = bearer.Split(" ").Last();
                var jsonToken = handler.ReadToken(token);
                return jsonToken as JwtSecurityToken;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}