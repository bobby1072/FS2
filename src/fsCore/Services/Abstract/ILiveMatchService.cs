using Common.Models;

namespace Services.Abstract
{
    public interface ILiveMatchService
    {
        Task<LiveMatch> CreateMatch(LiveMatch match, UserWithGroupPermissionSet currentUser);
        Task<LiveMatch> UpdateMatch(LiveMatch match, UserWithGroupPermissionSet currentUser);
        Task<LiveMatchCatch> SaveCatch(Guid matchId, LiveMatchCatch catches, UserWithGroupPermissionSet currentUser);
    }
}