using Common.Dbinterfaces.Repository;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;

namespace Persistence.EntityFramework.Repository
{
    internal class UserRepository : BaseRepository<UserEntity, User>, IUserRepository
    {
        public UserRepository(IDbContextFactory<FsContext> dbContextFactory) : base(dbContextFactory) { }
        protected override UserEntity _runtimeToEntity(User runtimeObj)
        {
            return UserEntity.RuntimeToEntity(runtimeObj);
        }
        public async Task<bool> IsUserNameUnique(User runtimeObj)
        {
            await using var context = await DbContextFactory.CreateDbContextAsync();
            var foundUsername = await context.User.FirstOrDefaultAsync(x => x.Username == runtimeObj.Username);
            return foundUsername is null;
        }
        public async Task<ICollection<UserWithoutEmail>> FindManyLikeWithSensitiveRemoved(string searchTerm)
        {
            await using var context = await DbContextFactory.CreateDbContextAsync();
            var foundUsersLike = await context
                .User
                .Where(x => x.Username.ToLower().Contains(searchTerm.ToLower()))
                .Take(10)
                .ToArrayAsync();
            return foundUsersLike?.Select(x => new UserWithoutEmail { EmailVerified = x.EmailVerified, Id = x.Id, Name = x.Name, Username = x.Username }).ToArray() ?? Array.Empty<UserWithoutEmail>();
        }
    }
}