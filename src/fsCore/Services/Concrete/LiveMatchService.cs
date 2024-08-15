using System.Net;
using Common;
using Common.Models;
using FluentValidation;
using fsCore.Services.Abstract;
using Services.Abstract;

namespace fsCore.Services.Concrete
{
    public class LiveMatchService : ILiveMatchService
    {
        private readonly IValidator<LiveMatch> _liveMatchValidator;
        private readonly ILiveMatchPersistenceService _liveMatchPersistenceService;
        public LiveMatchService(IValidator<LiveMatch> liveMatchValidator, ILiveMatchPersistenceService liveMatchPersistenceService)
        {
            _liveMatchValidator = liveMatchValidator;
            _liveMatchPersistenceService = liveMatchPersistenceService;
        }
        public async Task<bool> IsParticipant(Guid matchId, Guid userId)
        {
            return (await _liveMatchPersistenceService.TryGetLiveMatch(matchId) ?? throw new LiveMatchException(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails, HttpStatusCode.BadRequest)).Participants.Any(p => p.Id == userId);
        }
        public async Task<LiveMatch> CreateMatch(LiveMatch match, UserWithGroupPermissionSet currentUser)
        {

        }
        public async Task<LiveMatch> UpdateMatch(LiveMatch match, UserWithGroupPermissionSet currentUser);
        public async Task<ICollection<LiveMatchCatch>> SaveCatches(Guid matchId, ICollection<LiveMatchCatch> catches, UserWithGroupPermissionSet currentUser);
    }
}