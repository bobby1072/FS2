using Common.Models;

namespace Services.Abstract
{
    public interface ILiveMatchCachingService
    {
        Task<string> SetLiveMatch(LiveMatch liveMatch);
        Task<LiveMatch> GetLiveMatch(Guid matchId);
        Task<LiveMatch?> TryGetLiveMatch(Guid matchId);
    }
}