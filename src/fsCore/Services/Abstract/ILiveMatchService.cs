using Common.Models;

namespace fsCore.Services.Abstract
{
    public interface ILiveMatchService
    {
        Task<bool> IsParticipant(Guid matchId, Guid userId);
        Task<LiveMatch> CreateMatch(LiveMatch match, UserWithGroupPermissionSet currentUser);
        Task<LiveMatch> UpdateMatch(LiveMatch match, UserWithGroupPermissionSet currentUser);
        Task<LiveMatchCatch> SaveCatch(Guid matchId, LiveMatchCatch catches, UserWithGroupPermissionSet currentUser);
    }
}