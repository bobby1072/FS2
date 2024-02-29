using Common.Models;

namespace fsCore.Service
{
    public interface IUserInfoClient
    {
        Task<UserInfoResponse> GetUserInfo(string accessToken);
        Task<User> GetUserInfoReturnUser(string accessToken);
    }
}