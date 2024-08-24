using Common.Models;

namespace Services.Abstract
{
    public interface ILiveMatchPersistenceService
    {
        internal Task SaveCatch(Guid liveMatchId, LiveMatchCatch liveMatchCatch);
        internal Task SetLiveMatch(LiveMatch liveMatch);
        public Task<LiveMatch?> TryGetLiveMatch(Guid matchId);
    }
}