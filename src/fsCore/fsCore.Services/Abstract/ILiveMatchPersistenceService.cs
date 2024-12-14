using fsCore.Common.Models;

namespace fsCore.Services.Abstract
{
    public interface ILiveMatchPersistenceService
    {
        internal Task DeleteParticipant(Guid liveMatchId, LiveMatchParticipant user);
        public Task SaveParticipant(ICollection<(LiveMatchParticipant LiveMatchParticipant, Guid MatchId)> participants);
        internal Task DeleteCatch(Guid liveMatchId, LiveMatchCatch liveMatchCatch);
        internal Task SaveCatch(Guid liveMatchId, LiveMatchCatch liveMatchCatch);
        internal Task SetLiveMatch(LiveMatch liveMatch);
        public Task<LiveMatch?> TryGetLiveMatch(Guid matchId);
    }
}