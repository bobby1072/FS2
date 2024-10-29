using Common.Misc;
using Common.Models;
using Common.Utils;
using FluentValidation;
using Persistence.EntityFramework.Repository.Abstract;
using Services.Abstract;
using System.Net;

namespace Services.Concrete
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IValidator<User> _validator;
        public UserService(IUserRepository repository, IValidator<User> userValidator)
        {
            _validator = userValidator;
            _repo = repository;
        }
        public async Task<User> GetUser(Guid id, UserWithGroupPermissionSet currentUser)
        {
            var foundUser = await GetUser(id);
            if (currentUser.Id != foundUser.Id)
            {
                foundUser.RemoveSensitive();
            }
            return foundUser;
        }
        public async Task<ICollection<User>> GetUser(ICollection<Guid> id)
        {
            var foundUsers = await _repo.GetUsers(id);
            return foundUsers ?? throw new ApiException(ErrorConstants.NoUserFound, HttpStatusCode.NotFound);
        }
        public async Task<User> GetUser(Guid id)
        {
            var foundUser = await _repo.GetOne(id, typeof(User).GetProperty("Id".ToPascalCase())?.Name ?? throw new Exception());
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
        public async Task<string> FindUniqueUsername(User user)
        {
            var isUnique = false;
            while (!isUnique)
            {
                user.Username ??= user.CalculateDefaultUsername();
                isUnique = await _repo.IsUserNameUnique(user);
                if (!isUnique)
                {
                    user.Username = $"{user.Email.Split('@')[0]}{Guid.NewGuid()}";
                }
            }
            return user.Username!;
        }
        public async Task<User> SaveUser(User user)
        {
            await _validator.ValidateAndThrowAsync(user);
            var foundUser = await _repo.GetOne(user.Email, "email".ToPascalCase());
            if (foundUser != null)
            {
                if (!user.ValidateAgainstOriginal(foundUser)) throw new ApiException(ErrorConstants.NotAllowedToEditThoseFields, HttpStatusCode.BadRequest);
                return (await _repo.Update(new List<User> { user }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CantCreateUser, HttpStatusCode.InternalServerError);
            }
            return (await _repo.Create(new List<User> { user }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CantCreateUser, HttpStatusCode.InternalServerError);
        }
        public async Task<User> CheckUserExistsAndCreateIfNot(User user)
        {
            var foundUser = await _repo.GetOne(user.Email, "email".ToPascalCase());
            if (foundUser is not null)
            {
                return foundUser;
            }
            user.Username = await FindUniqueUsername(user);
            await _validator.ValidateAndThrowAsync(user);
            return (await _repo.Create(new List<User> { user }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CantCreateUser, HttpStatusCode.InternalServerError);
        }
        public async Task<ICollection<UserWithoutEmail>> SearchUsers(string searchTerm)
        {
            var foundUsers = await _repo.FindManyLikeWithSensitiveRemoved(searchTerm);
            return foundUsers;
        }
    }
}