using Common.Models;

namespace Services.Abstract
{
    public interface IUserInfoClient
    {
        Task<User> GetUserInfoReturnUser(string accessToken);
    }
}