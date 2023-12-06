using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Common.Authentication;
using Common.Models;
using Microsoft.Extensions.Options;

namespace fsCore.Contexts
{
    public class UserInfoClient : IUserInfoClient
    {
        private readonly string _userInfoEndpoint;
        private HttpClient _client;
        public UserInfoClient(HttpClient httpClient, IOptions<ClientConfigSettings> options)
        {
            _userInfoEndpoint = options.Value.UserInfoEndpoint;
            _client = httpClient;
        }
        public async Task<UserInfoResponse> GetUserInfo(string accessToken)
        {
            using var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_userInfoEndpoint),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), accessToken},
                }
            };

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<IDictionary<string, string>>(content)
                       ?? throw new InvalidOperationException("Could not deserialise user info data");

            var claims = data.Select(kvp => new Claim(kvp.Key, kvp.Value));
            return new UserInfoResponse(claims);
        }
        public async Task<User> GetUserInfoReturnUser(string accessToken)
        {
            var response = await GetUserInfo(accessToken);
            var email = response.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value
                        ?? throw new InvalidOperationException("User does not have email claim");
            var emailVerified = response.Claims.FirstOrDefault(claim => claim.Type == "email_verified")?.Value
                                ?? throw new InvalidOperationException("User does not have email_verified claim");
            var name = response.Claims.FirstOrDefault(claim => claim.Type == "name")?.Value
                       ?? throw new InvalidOperationException("User does not have name claim");
            return new User(email, bool.Parse(emailVerified), name);
        }
    }
    public class UserInfoResponse
    {
        public UserInfoResponse(IEnumerable<Claim> claims)
        {
            Claims = claims;
        }

        public IEnumerable<Claim> Claims { get; }
    }
}