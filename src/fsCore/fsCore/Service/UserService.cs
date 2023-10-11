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
            var userProperties = typeof(WorldFish).GetProperties();
            var foundDetail = userProperties.FirstOrDefault(x =>
            {
                var worldFishPropertyType = x.GetType();
                return x.Name == "email".ToPascalCase() && typeof(string) == x.PropertyType;
            }) ?? throw new ApiException(ErrorConstants.FieldNotFound, HttpStatusCode.NotFound);
            var foundUser = await _repo.GetOne(email, "email".ToPascalCase());
            return foundUser ?? throw new ApiException(ErrorConstants.NoUserFound, HttpStatusCode.NotFound);
        }
        public async Task<User> MakeSureUserExists(User user)
        {
            var foundUser = await _repo.GetOne(user);
            if (foundUser is null)
            {
                var createdEntities = await _repo.Create(new List<User> { user });
                if (createdEntities is null || !createdEntities.Any())
                    throw new ApiException(ErrorConstants.CantCreateUser, HttpStatusCode.NotFound);
                return createdEntities.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CantCreateUser, HttpStatusCode.NotFound);
            }
            return foundUser;
        }
    }
}