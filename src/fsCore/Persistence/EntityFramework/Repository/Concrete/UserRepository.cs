using Common.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;
using Persistence.EntityFramework.Repository.Abstract;

namespace Persistence.EntityFramework.Repository.Concrete
{
    internal class UserRepository : BaseRepository<UserEntity, User>, IUserRepository
    {
        public UserRepository(IDbContextFactory<FsContext> dbContextFactory) : base(dbContextFactory) { }
        protected override UserEntity RuntimeToEntity(User runtimeObj)
        {
            return UserEntity.RuntimeToEntity(runtimeObj);
        }
        public async Task<ICollection<User>> GetUsers(ICollection<Guid> ids)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var foundUsers = await context.User.Where(x => ids.Contains(x.Id)).ToArrayAsync();
            return foundUsers.Select(x => x.ToRuntime()).ToArray();
        }
        public async Task<bool> IsUserNameUnique(User runtimeObj)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var foundUsername = await context.User.FirstOrDefaultAsync(x => x.Username == runtimeObj.Username);
            return foundUsername is null;
        }
        public async Task<ICollection<UserWithoutEmail>> FindManyLikeWithSensitiveRemoved(string searchTerm)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var foundUsersLike = await context
                .User
                .Where(x => x.Username.ToLower().Contains(searchTerm.ToLower()))
                .Take(30)
                .ToArrayAsync();
            return foundUsersLike?.Select(x => new UserWithoutEmail { EmailVerified = x.EmailVerified, Id = x.Id, Name = x.Name, Username = x.Username }).ToArray() ?? Array.Empty<UserWithoutEmail>();
        }
    }
}