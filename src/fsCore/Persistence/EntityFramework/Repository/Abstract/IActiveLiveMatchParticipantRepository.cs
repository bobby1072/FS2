using Common.Models;

namespace Persistence.EntityFramework.Repository.Abstract
{
    public interface IActiveLiveMatchParticipantRepository
    {
        Task<ICollection<User>?> Create(ICollection<User> runtimeObjs, Guid matchId);
        Task<User?> Delete(User runtimeObj, Guid matchId);
    }
}