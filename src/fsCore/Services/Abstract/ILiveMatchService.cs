using Common.Models;

namespace Services.Abstract
{
    public interface ILiveMatchService
    {
        Task<LiveMatch> CreateMatch(LiveMatch match, UserWithGroupPermissionSet currentUser, string tokenString);
        Task UpdateMatch(LiveMatch match, UserWithGroupPermissionSet currentUser);
        Task<LiveMatchCatch> SaveCatch(Guid matchId, LiveMatchCatch catches, UserWithGroupPermissionSet currentUser);
        Task<LiveMatch> StartMatch(Guid matchId, UserWithGroupPermissionSet userWithGroupPermissionSet, bool shouldUpdateClients = false);
        Task<LiveMatch> EndMatch(Guid matchId, UserWithGroupPermissionSet userWithGroupPermissionSet, bool shouldUpdateClients = false);
    }
}