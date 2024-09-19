using Common.Models;

namespace Services.Abstract
{
    public interface ILiveMatchPersistenceService
    {
        Task DeleteParticipant(Guid liveMatchId, User user);
        Task SaveParticipant(Guid liveMatchId, User user);
        Task SaveParticipant(Guid liveMatchId, ICollection<User> user);
        Task DeleteCatch(Guid liveMatchId, LiveMatchCatch liveMatchCatch);
        Task SaveCatch(Guid liveMatchId, LiveMatchCatch liveMatchCatch);
        Task SetLiveMatch(LiveMatch liveMatch);
        Task<LiveMatch?> TryGetLiveMatch(Guid matchId);
        Task<ICollection<Guid>> AllLiveMatchesForUser(Guid userId);
    }
}