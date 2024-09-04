using Common.Models;

namespace Services.Abstract
{
    public interface ILiveMatchPersistenceService
    {
        internal Task DeleteParticipant(Guid liveMatchId, User user);
        internal Task SaveParticipant(Guid liveMatchId, User user);
        internal Task DeleteCatch(Guid liveMatchId, LiveMatchCatch liveMatchCatch);
        internal Task SaveCatch(Guid liveMatchId, LiveMatchCatch liveMatchCatch);
        internal Task SetLiveMatch(LiveMatch liveMatch);
        public Task<LiveMatch?> TryGetLiveMatch(Guid matchId);
    }
}