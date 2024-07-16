using Common.Models;
using fsCore.Services.Concrete;

namespace fsCore.Services.Abstract
{
    public interface IUserInfoClient
    {
        Task<UserInfoResponse> GetUserInfo(string accessToken);
        Task<User> GetUserInfoReturnUser(string accessToken);
    }
}