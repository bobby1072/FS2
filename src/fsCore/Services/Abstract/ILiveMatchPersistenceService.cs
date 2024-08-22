using Common.Models;

namespace Services.Abstract
{
    public interface ILiveMatchPersistenceService
    {
        Task SaveCatch(Guid liveMatchId, LiveMatchCatch liveMatchCatch);
        Task SetLiveMatch(LiveMatch liveMatch);
        Task<LiveMatch?> TryGetLiveMatch(Guid matchId);
    }
}