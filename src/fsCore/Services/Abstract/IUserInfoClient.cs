using Common.Models;
using Services.Concrete;

namespace Services.Abstract
{
    public interface IUserInfoClient
    {
        Task<User> GetUserInfoReturnUser(string accessToken);
    }
}