using Common.Models;

namespace Persistence.EntityFramework.Repository.Abstract
{
    public interface IActiveLiveMatchParticipantRepository
    {
        Task<ICollection<LiveMatchParticipant>?> Update(ICollection<LiveMatchParticipant> newRuntimeObjs, Guid matchId);
        Task<ICollection<Guid>> GetMatchIdsForUser(Guid userId);
        Task<User?> Delete(ICollection<Guid> userIdList, Guid matchId);
        Task<ICollection<LiveMatchParticipant>?> Create(ICollection<LiveMatchParticipant> runtimeObjs, Guid matchId);
        Task<User?> Delete(User runtimeObj, Guid matchId);
        Task<ICollection<LiveMatchParticipant>?> GetForMatch(Guid matchId);
    }
}