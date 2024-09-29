using Common.Authentication;
using Common.Models;
using Microsoft.Extensions.Options;
using Services.Abstract;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace Services.Concrete
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
        private async Task<UserInfoResponse> GetUserInfo(string accessToken)
        {
            using var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_userInfoEndpoint),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), accessToken.Split("Bearer ").Length == 2 ? accessToken : $"Bearer {accessToken}"},
                }
            };

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(content)
                       ?? throw new InvalidOperationException("Could not deserialise user info data");

            var claims = data.Select(kvp => new Claim(kvp.Key, kvp.Value));
            return new UserInfoResponse(claims);
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