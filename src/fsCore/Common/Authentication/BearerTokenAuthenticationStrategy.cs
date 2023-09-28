using Microsoft.AspNetCore.Http;
using System.Net;

namespace Common.Authentication

{
    public class BearerTokenAuthenticationStrategy : IBearerTokenAuthenticationStrategy
    {
        public AuthenticationStrategy Type { get; }
        private readonly IHttpContextAccessor _httpContextAccessor;
        public const string ExpiryClaimKey = "exp";
        public BearerTokenAuthenticationStrategy(IHttpContextAccessor httpContextAccessor)
        {
            Type = AuthenticationStrategy.BearerToken;
            _httpContextAccessor = httpContextAccessor;
        }
        private HttpContext GetHttpContext()
        {
            return _httpContextAccessor.HttpContext ?? throw new InvalidOperationException($"{nameof(_httpContextAccessor.HttpContext)} is null");
        }
        public long? TokenExpiryInSecond
        {
            get
            {
                var value = GetHttpContext().User.Claims.FirstOrDefault(claim => claim.Type == ExpiryClaimKey)?.Value;
                if (value == null) return null;
                return Math.Abs(long.Parse(value) - DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            }
        }
        public Tuple<string, string> AuthenticationHeader
        {
            get
            {
                var authHeaderValue = GetHttpContext().GetAuthorizationHeaderValue();
                if (authHeaderValue == null)
                {
                    throw new InvalidOperationException("No authorization header found");
                }
                return Tuple.Create(HttpRequestHeader.Authorization.ToString(), authHeaderValue);
            }
        }
    }
}
