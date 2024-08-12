using Common.Models;

namespace fsCore.Hubs.Abstract
{
    public interface ILiveMatchHub
    {
        Task OnConnectedAsync();
        Task UpdateLiveMatch(LiveMatch liveMatch);
    }
}