using Common.Models;

namespace Services.Abstract
{
    public interface ILiveMatchPersistenceService
    {
        Task SetLiveMatchCatches(Guid liveMatchId, ICollection<LiveMatchCatch> liveMatchCatches);
        Task SetLiveMatch(LiveMatch liveMatch);
        Task<LiveMatch?> TryGetLiveMatch(Guid matchId);
    }
}