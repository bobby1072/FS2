using Common.Models;

namespace Persistence.EntityFramework.Repository.Abstract
{
    public interface IActiveLiveMatchParticipantRepository
    {
        Task<User?> Delete(ICollection<Guid> userIdList, Guid matchId);
        Task<ICollection<User>?> Create(ICollection<User> runtimeObjs, Guid matchId);
        Task<User?> Delete(User runtimeObj, Guid matchId);
        Task<ICollection<User>?> GetForMatch(Guid matchId);
    }
}