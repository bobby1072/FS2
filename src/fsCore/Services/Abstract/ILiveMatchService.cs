using Common.Models;

namespace fsCore.Services.Abstract
{
    public interface ILiveMatchService
    {
        Task<bool> IsParticipant(Guid matchId, Guid userId);
        Task<LiveMatch> CreateMatch(LiveMatch match, UserWithGroupPermissionSet currentUser);
        Task<LiveMatch> UpdateMatch(LiveMatch match, UserWithGroupPermissionSet currentUser);
        Task<ICollection<LiveMatchCatch>> SaveCatches(Guid matchId, ICollection<LiveMatchCatch> catches, UserWithGroupPermissionSet currentUser);
    }
}