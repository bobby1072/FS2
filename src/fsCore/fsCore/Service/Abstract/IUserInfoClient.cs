using Common.Models;
using fsCore.Service.Concrete;

namespace fsCore.Service.Abstract
{
    public interface IUserInfoClient
    {
        Task<UserInfoResponse> GetUserInfo(string accessToken);
        Task<User> GetUserInfoReturnUser(string accessToken);
    }
}