using System.Net;
using Common;
using Common.Models;
using Common.Permissions;
using FluentValidation;
using Services.Abstract;

namespace Services.Concrete
{
    public class LiveMatchService : ILiveMatchService
    {
        private readonly ILiveMatchPersistenceService _liveMatchPersistenceService;
        private readonly IValidator<LiveMatch> _liveMatchValidator;
        private readonly IValidator<LiveMatchCatch> _liveMatchCatchValidator;
        public LiveMatchService(ILiveMatchPersistenceService liveMatchPersistenceService,
            IValidator<LiveMatch> liveMatchValidator,
            IValidator<LiveMatchCatch> liveMatchCatchValidator
            )
        {
            _liveMatchValidator = liveMatchValidator;
            _liveMatchCatchValidator = liveMatchCatchValidator;
            _liveMatchPersistenceService = liveMatchPersistenceService;
        }
        public async Task<bool> IsParticipant(Guid matchId, Guid userId)
        {
            return (await _liveMatchPersistenceService.TryGetLiveMatch(matchId) ?? throw new LiveMatchException(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails, HttpStatusCode.BadRequest)).Participants.Any(p => p.Id == userId);
        }
        public async Task<LiveMatch> CreateMatch(LiveMatch match, UserWithGroupPermissionSet currentUser)
        {
            if (!currentUser.GroupPermissions.Can(PermissionConstants.Manage, match.GroupId))
            {
                throw new LiveMatchException(ErrorConstants.DontHavePermission, HttpStatusCode.Unauthorized);
            }

            var foundMatch = await _liveMatchPersistenceService.TryGetLiveMatch(match.Id);
            if (foundMatch is not null)
            {
                throw new LiveMatchException(LiveMatchConstants.LiveMatchAlreadyExists, HttpStatusCode.Conflict);
            }

            match.ApplyDefaults(LiveMatchStatus.NotStarted, currentUser);
            await _liveMatchValidator.ValidateAndThrowAsync(match);
            await _liveMatchPersistenceService.SetLiveMatch(match);
            return match;

        }
        public async Task<LiveMatch> UpdateMatch(LiveMatch match, UserWithGroupPermissionSet currentUser)
        {
            var foundMatch = await _liveMatchPersistenceService.TryGetLiveMatch(match.Id) ?? throw new LiveMatchException(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails, HttpStatusCode.BadRequest);
            if (foundMatch.EndsAt is not null && foundMatch.EndsAt < DateTime.UtcNow)
            {
                throw new LiveMatchException(LiveMatchConstants.LiveMatchHasEnded, HttpStatusCode.BadRequest);
            }
            if (!foundMatch.Catches.SequenceEqual(match.Catches))
            {
                throw new LiveMatchException(LiveMatchConstants.FailedToPersistLiveMatch, HttpStatusCode.BadRequest);
            }
            if (!currentUser.GroupPermissions.Can(PermissionConstants.Manage, foundMatch.GroupId))
            {
                throw new LiveMatchException(ErrorConstants.DontHavePermission, HttpStatusCode.Unauthorized);
            }
            match.ApplyDefaults(LiveMatchStatus.InProgress, currentUser);
            if (currentUser.Id != foundMatch.MatchLeaderId)
            {
                throw new LiveMatchException(ErrorConstants.DontHavePermission, HttpStatusCode.Unauthorized);
            }
            if (!match.ValidateAgainstOriginal(foundMatch))
            {
                throw new LiveMatchException(ErrorConstants.NotAllowedToEditThoseFields, HttpStatusCode.Forbidden);
            }

            await _liveMatchValidator.ValidateAndThrowAsync(match);
            await _liveMatchPersistenceService.SetLiveMatch(match);
            return match;
        }
        public async Task<LiveMatchCatch> SaveCatch(Guid matchId, LiveMatchCatch liveMatchCatch, UserWithGroupPermissionSet currentUser)
        {
            var foundMatch = await _liveMatchPersistenceService.TryGetLiveMatch(matchId) ?? throw new LiveMatchException(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails, HttpStatusCode.BadRequest);
            if (foundMatch.EndsAt is not null && foundMatch.EndsAt < DateTime.UtcNow)
            {
                throw new LiveMatchException(LiveMatchConstants.LiveMatchHasEnded, HttpStatusCode.BadRequest);
            }
            else if (foundMatch.CommencesAt is not null && foundMatch.CommencesAt > DateTime.UtcNow)
            {
                throw new LiveMatchException(LiveMatchConstants.LiveMatchHasntStarted, HttpStatusCode.BadRequest);
            }
            if (!foundMatch.Participants.Any(x => x.Id == currentUser.Id))
            {
                throw new LiveMatchException(ErrorConstants.DontHavePermission, HttpStatusCode.Unauthorized);
            }

            if (currentUser.Id != liveMatchCatch.UserId && currentUser.Id != foundMatch.MatchLeaderId)
            {
                throw new LiveMatchException(ErrorConstants.DontHavePermission, HttpStatusCode.Unauthorized);
            }

            var dynamicCatchValidator = foundMatch.MatchRules.BuildMatchCatchValidator();

            var isCatchValidJob = dynamicCatchValidator.ValidateAsync(liveMatchCatch);
            await Task.WhenAll(_liveMatchCatchValidator.ValidateAndThrowAsync(liveMatchCatch), isCatchValidJob);

            var isCatchValid = (await isCatchValidJob).IsValid;
            if (foundMatch.Catches.FirstOrDefault(x => x.Id == liveMatchCatch.Id) is LiveMatchCatch foundCatch)
            {
                if (!liveMatchCatch.ValidateAgainstOriginal(foundCatch))
                {
                    throw new LiveMatchException(ErrorConstants.DontHavePermission, HttpStatusCode.Unauthorized);
                }
            }
            if (liveMatchCatch.Id is null)
            {
                liveMatchCatch.ApplyDefaults();
                liveMatchCatch.CountsInMatch = isCatchValid;
                await _liveMatchPersistenceService.SaveCatch(foundMatch.Id, liveMatchCatch);
                return liveMatchCatch;
            }
            else
            {
                liveMatchCatch.CountsInMatch = isCatchValid;
                await _liveMatchPersistenceService.SaveCatch(foundMatch.Id, liveMatchCatch);
                return liveMatchCatch;
            }
        }
    }
}