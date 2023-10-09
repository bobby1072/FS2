using Common.Dbinterfaces.Repository;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityFramework.Entity;

namespace Persistence.EntityFramework.Repository
{
    internal class UserRepository : BaseRepository<UserEntity, User>, IUserRepository
    {
        public UserRepository(IDbContextFactory<FsContext> dbContextFactory) : base(dbContextFactory) { }
        public Task<ICollection<User>?> Create(ICollection<User> userToCreate) => _create(userToCreate.Select(x => UserEntity.RuntimeToEntity(x)).ToList());
        public Task<ICollection<User>?> Update(ICollection<User> userToCreate) => _update(userToCreate.Select(x => UserEntity.RuntimeToEntity(x)).ToList());
        public Task<ICollection<User>?> Delete(ICollection<User> userToCreate) => _delete(userToCreate.Select(x => UserEntity.RuntimeToEntity(x)).ToList());

    }
}