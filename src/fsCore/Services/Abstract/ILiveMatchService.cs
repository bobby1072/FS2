using Common.Models;

namespace Services.Abstract
{
    public interface ILiveMatchService
    {
        Task<LiveMatch> CreateMatch(LiveMatch match, UserWithGroupPermissionSet currentUser);
        Task UpdateMatch(LiveMatch match, UserWithGroupPermissionSet currentUser);
        Task<LiveMatchCatch> SaveCatch(Guid matchId, LiveMatchCatch catches, UserWithGroupPermissionSet currentUser);
        Task<LiveMatch> StartMatch(Guid matchId, bool shouldUpdateClients = false);
        Task<LiveMatch> EndMatch(Guid matchId, bool shouldUpdateClients = false);
    }
}