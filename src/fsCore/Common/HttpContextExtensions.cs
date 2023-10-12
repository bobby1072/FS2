using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace Common
{
    public static class HttpContextExtensions
    {
        public const string WebsocketAuthKey = "websocket-auth-token";
        private const string Prefix = "Bearer ";

        public static string? GetAuthorizationHeaderValue(this HttpContext httpContext, bool includePrefix = true)
        {
            string authHeaderValue;
            if (httpContext.WebSockets.IsWebSocketRequest)
            {
                if (httpContext.Items.TryGetValue(WebsocketAuthKey, out var token) &&
                    token is string stringToken)
                {
                    authHeaderValue = stringToken;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                var authHeader = httpContext.Request.Headers[HttpRequestHeader.Authorization.ToString()];
                if (authHeader.Count == 0)
                {
                    return null;
                }

                authHeaderValue = authHeader.First();
            }

            if (includePrefix)
            {
                return authHeaderValue;
            }

            return SplitTokenOnPrefix(authHeaderValue)?.LastOrDefault();
        }

        public static string[]? SplitTokenOnPrefix(string token)
        {
            var splits = token.Split(Prefix);
            if (splits.Length == 1)
            {
                return null;
            }

            if (splits.Length > 2)
            {
                throw new InvalidOperationException($"Expected 1 prefix got {splits.Length}");
            }

            return splits;
        }
    }
}
