using Common.Models;

namespace fsCore.Hubs.Abstract
{
    public interface ILiveMatchHub
    {
        Task OnConnectedAsync();
        Task SaveLiveMatch(LiveMatch liveMatch);
        Task SaveLiveMatchCatch(LiveMatchCatch liveMatchCatch);
    }
}