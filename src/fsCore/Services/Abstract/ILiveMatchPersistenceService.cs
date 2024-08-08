using Common.Models;

namespace Services.Abstract
{
    public interface ILiveMatchPersistenceService
    {
        Task<string> SetLiveMatch(LiveMatch liveMatch);
        Task<LiveMatch?> TryGetLiveMatch(Guid matchId);
    }
}