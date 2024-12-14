using fsCore.Common.Models;

namespace fsCore.Services.Abstract
{
    public interface IUserInfoClient
    {
        Task<User> GetUserInfoReturnUser(string accessToken);
    }
}