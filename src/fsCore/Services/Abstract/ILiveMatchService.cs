using Common.Models;

namespace Services.Abstract
{
    public interface ILiveMatchService
    {
        Task<ICollection<LiveMatch>> AllMatchesParticipatedIn(ICollection<Guid> matchIds);
        Task<ICollection<Guid>> AllMatchesParticipatedIn(UserWithGroupPermissionSet currentUser);
        Task CreateParticipant(Guid matchId, Guid userId, UserWithGroupPermissionSet currentUser);
        Task<LiveMatch> CreateMatch(LiveMatch match, UserWithGroupPermissionSet currentUser);
        Task UpdateMatch(LiveMatch match, UserWithGroupPermissionSet currentUser);
        Task SaveCatch(Guid matchId, LiveMatchCatch catches, UserWithGroupPermissionSet currentUser);
        Task<LiveMatch> StartMatch(Guid matchId, UserWithGroupPermissionSet userWithGroupPermissionSet);
        Task<LiveMatch> EndMatch(Guid matchId, UserWithGroupPermissionSet userWithGroupPermissionSet);
    }
}