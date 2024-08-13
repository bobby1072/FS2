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
        public async Task<LiveMatch> SaveMatch(LiveMatch match, UserWithGroupPermissionSet currentUser)
        {
            throw new NotImplementedException();
        }
        // private async Task ValidateMatch(LiveMatch match)
        // {
        //     await _liveMatchValidator.ValidateAndThrowAsync(match);
        //     var dynamicCatchValidator = match.MatchRules.BuildMatchRulesValidator();
        //     foreach (var matchCatch in match.Catches)
        //     {
        //         dynamicCatchValidator.ValidateAndThrowAsync(matchCatch);
        //     }
        // }
    }
}