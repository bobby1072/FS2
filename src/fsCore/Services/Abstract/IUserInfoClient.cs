using Common.Models;
using Services.Concrete;

namespace Services.Abstract
{
    public interface IUserInfoClient
    {
        Task<UserInfoResponse> GetUserInfo(string accessToken);
        Task<User> GetUserInfoReturnUser(string accessToken);
    }
}