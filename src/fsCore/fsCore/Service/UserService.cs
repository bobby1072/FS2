using System.Net;
using Common;
using Common.Dbinterfaces.Repository;
using Common.Models;
using Common.Utils;

namespace fsCore.Service
{
    public class UserService : BaseService<User, IUserRepository>, IUserService
    {
        public UserService(IUserRepository repository) : base(repository) { }
        public async Task<User> GetUser(User user)
        {
            var foundUser = await _repo.GetOne(user);
            return foundUser ?? throw new ApiException(ErrorConstants.NoUserFound, HttpStatusCode.NotFound);
        }
        public async Task<User> GetUser(string email)
        {
            var userProperties = typeof(User).GetProperties();
            var foundDetail = userProperties.FirstOrDefault(x =>
            {
                var worldFishPropertyType = x.GetType();
                return x.Name == "email".ToPascalCase() && typeof(string) == x.PropertyType;
            }) ?? throw new ApiException(ErrorConstants.FieldNotFound, HttpStatusCode.NotFound);
            var foundUser = await _repo.GetOne(email, "email".ToPascalCase());
            return foundUser ?? throw new ApiException(ErrorConstants.NoUserFound, HttpStatusCode.NotFound);
        }
        public async Task<User> CreateUser(User user)
        {
            var foundUser = await _repo.GetOne(user);
            if (foundUser != null)
            {
                throw new ApiException(ErrorConstants.UserAlreadyExists, HttpStatusCode.Conflict);
            }
            return (await _repo.Create(new List<User> { user }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CantCreateUser, HttpStatusCode.InternalServerError);
        }
        public async Task<User> UpdateUser(User user)
        {
            var foundUser = await _repo.GetOne(user) ?? throw new ApiException(ErrorConstants.NoUserFound, HttpStatusCode.NotFound);
            return (await _repo.Update(new List<User> { user }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CantCreateUser, HttpStatusCode.InternalServerError);
        }
        public async Task<User> DeleteUser(User user)
        {
            var foundUser = await _repo.GetOne(user) ?? throw new ApiException(ErrorConstants.NoUserFound, HttpStatusCode.NotFound);
            return (await _repo.Delete(new List<User> { user }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CantCreateUser, HttpStatusCode.InternalServerError);
        }
        public async Task<bool> Exists(string email)
        {
            var userProperties = typeof(User).GetProperties();
            var foundDetail = userProperties.FirstOrDefault(x =>
            {
                var worldFishPropertyType = x.GetType();
                return x.Name == "email".ToPascalCase() && typeof(string) == x.PropertyType;
            }) ?? throw new ApiException(ErrorConstants.FieldNotFound, HttpStatusCode.NotFound);
            var foundUser = await _repo.GetOne(email, "email".ToPascalCase());
            return foundUser != null;

        }
        public async Task<bool> Exists(User user)
        {
            var foundUser = await _repo.GetOne(user);
            return foundUser != null;
        }
        public async Task<bool> ExistsAndVerified(string email)
        {
            var userProperties = typeof(User).GetProperties();
            var foundDetail = userProperties.FirstOrDefault(x =>
            {
                var worldFishPropertyType = x.GetType();
                return x.Name == "email".ToPascalCase() && typeof(string) == x.PropertyType;
            }) ?? throw new ApiException(ErrorConstants.FieldNotFound, HttpStatusCode.NotFound);
            var foundUser = await _repo.GetOne(email, "email".ToPascalCase());
            return foundUser != null && foundUser.EmailVerified;
        }
        public async Task<bool> ExistsAndVerified(User user)
        {
            var foundUser = await _repo.GetOne(user);
            return foundUser != null && foundUser.EmailVerified;
        }
        public async Task<User> CheckUserExistsAndCreateIfNot(User user)
        {
            var foundUser = await _repo.GetOne(user);
            if (foundUser != null)
            {
                return foundUser;
            }
            return (await _repo.Create(new List<User> { user }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CantCreateUser, HttpStatusCode.InternalServerError);
        }
    }
}