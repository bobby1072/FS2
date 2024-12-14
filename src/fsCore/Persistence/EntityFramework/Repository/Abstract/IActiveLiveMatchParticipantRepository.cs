using fsCore.Common.Models;

namespace Persistence.EntityFramework.Repository.Abstract
{
    public interface IActiveLiveMatchParticipantRepository
    {
        Task<ICollection<LiveMatchParticipant>?> Update(ICollection<LiveMatchParticipant> newRuntimeObjs, Guid matchId);
        Task<ICollection<LiveMatchParticipant>?> Update(ICollection<Guid> matchIds, LiveMatchParticipant runtimeObj);
        Task<ICollection<LiveMatchParticipant>?> Update(ICollection<(LiveMatchParticipant LiveMatchParticipant, Guid MatchId)> participants);
        Task<ICollection<LiveMatchParticipant>?> Create(ICollection<(LiveMatchParticipant LiveMatchParticipant, Guid MatchId)> participants);
        Task<ICollection<Guid>> GetMatchIdsForUser(Guid userId);
        Task<ICollection<LiveMatchParticipant>?> Create(ICollection<LiveMatchParticipant> runtimeObjs, Guid matchId);
        Task Delete(ICollection<Guid> userIdList, Guid matchId);
        Task Delete(User runtimeObj, Guid matchId);
        Task Delete(ICollection<LiveMatchParticipant> participants);
        Task<ICollection<LiveMatchParticipant>?> GetForMatch(Guid matchId);
        Task<ICollection<LiveMatchParticipant>?> GetForMatch(ICollection<Guid> matchId);
    }
}