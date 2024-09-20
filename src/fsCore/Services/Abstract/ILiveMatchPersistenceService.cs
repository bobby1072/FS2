using Common.Models;

namespace Services.Abstract
{
    public interface ILiveMatchPersistenceService
    {
        Task DeleteParticipant(Guid liveMatchId, LiveMatchParticipant user);
        Task SaveParticipant(Guid liveMatchId, LiveMatchParticipant user);
        Task SaveParticipant(Guid liveMatchId, ICollection<LiveMatchParticipant> user);
        Task DeleteCatch(Guid liveMatchId, LiveMatchCatch liveMatchCatch);
        Task SaveCatch(Guid liveMatchId, LiveMatchCatch liveMatchCatch);
        Task SetLiveMatch(LiveMatch liveMatch);
        Task<LiveMatch?> TryGetLiveMatch(Guid matchId);
    }
}