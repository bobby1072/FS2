using Common.Authentication;
using Common.Models;
using Microsoft.Extensions.Options;
using Services.Abstract;
using System.Net;
using System.Net.Http.Json;

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

            return new User(response.Email, bool.Parse(response.EmailVerified), response.Name);
        }

        private async Task<UserInfoResponse> GetUserInfo(string accessToken)
        {
            using var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_userInfoEndpoint),
                Headers =
                {
                    {
                        HttpRequestHeader.Authorization.ToString(),
                        accessToken.Split("Bearer ").Length == 2
                            ? accessToken
                            : $"Bearer {accessToken}"
                    },
                },
            };

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadFromJsonAsync<UserInfoResponse>();

            return jsonResponse
                ?? throw new InvalidOperationException("Failed to deserialize UserInfo response");
        }
    }
}
