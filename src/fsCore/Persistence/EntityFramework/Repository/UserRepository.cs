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
            await using var context = DbContextFactory.CreateDbContext();
            var foundUsername = await context.User.FirstOrDefaultAsync(x => x.Username == runtimeObj.Username);
            return foundUsername is null;
        }
    }
}