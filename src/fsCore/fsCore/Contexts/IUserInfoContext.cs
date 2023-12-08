using Common.Models;

namespace fsCore.Contexts
{
    public interface IUserInfoClient
    {
        Task<UserInfoResponse> GetUserInfo(string accessToken);
        Task<User> GetUserInfoReturnUser(string accessToken);
    }
}