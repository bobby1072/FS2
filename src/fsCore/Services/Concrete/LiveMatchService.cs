using System.Net;
using Common;
using Common.Misc.Abstract;
using Common.Models;
using Common.Permissions;
using FluentValidation;
using Hangfire;
using Services.Abstract;

namespace Services.Concrete
{
    public class LiveMatchService : ILiveMatchService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILiveMatchHubContextServiceProvider _liveMatchHubContextServiceProvider;
        private readonly ILiveMatchPersistenceService _liveMatchPersistenceService;
        private readonly IValidator<LiveMatch> _liveMatchValidator;
        private readonly IValidator<LiveMatchCatch> _liveMatchCatchValidator;
        public LiveMatchService(IBackgroundJobClient backgroundJobClient,
            ILiveMatchHubContextServiceProvider liveMatchHubContextServiceProvider,
            ILiveMatchPersistenceService liveMatchPersistenceService,
            IValidator<LiveMatch> liveMatchValidator,
            IValidator<LiveMatchCatch> liveMatchCatchValidator
            )
        {
            _backgroundJobClient = backgroundJobClient;
            _liveMatchHubContextServiceProvider = liveMatchHubContextServiceProvider;
            _liveMatchPersistenceService = liveMatchPersistenceService;
            _liveMatchCatchValidator = liveMatchCatchValidator;
            _liveMatchValidator = liveMatchValidator;
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

            if (match.CommencesAt is DateTime commenceTime)
            {
                var timeSpanBetweenNowAndWhenToExecuteJob = commenceTime.Subtract(DateTime.UtcNow);
                _backgroundJobClient.Schedule(() => StartMatch(match.Id, true), timeSpanBetweenNowAndWhenToExecuteJob);
            }
            if (match.EndsAt is DateTime endTime)
            {
                var timeSpanBetweenNowAndWhenToExecuteJob = endTime.Subtract(DateTime.UtcNow);
                _backgroundJobClient.Schedule(() => EndMatch(match.Id, true), timeSpanBetweenNowAndWhenToExecuteJob);
            }

            return match;

        }
        public async Task UpdateMatch(LiveMatch match, UserWithGroupPermissionSet currentUser)
        {
            match.Catches = [];
            var foundMatch = await _liveMatchPersistenceService.TryGetLiveMatch(match.Id) ?? throw new LiveMatchException(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails, HttpStatusCode.BadRequest);
            match.MatchStatus = foundMatch.MatchStatus;
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
        [Queue(HangfireConstants.Queues.StartUpJobs)]
        [AutomaticRetry(Attempts = 3, LogEvents = true, DelaysInSeconds = [1], OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public async Task<LiveMatch> StartMatch(Guid matchId, bool shouldUpdateClients = false)
        {
            var foundMatch = await _liveMatchPersistenceService.TryGetLiveMatch(matchId) ?? throw new LiveMatchException(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails, HttpStatusCode.BadRequest);
            foundMatch.MatchStatus = LiveMatchStatus.InProgress;
            foundMatch.CommencesAt = DateTime.UtcNow;
            await _liveMatchPersistenceService.SetLiveMatch(foundMatch);
            if (shouldUpdateClients)
            {
                await _liveMatchHubContextServiceProvider.UpdateMatchForClients(matchId);
            }
            return foundMatch;
        }
        [Queue(HangfireConstants.Queues.StartUpJobs)]
        [AutomaticRetry(Attempts = 3, LogEvents = true, DelaysInSeconds = [1], OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public async Task<LiveMatch> EndMatch(Guid matchId, bool shouldUpdateClients = false)
        {
            var foundMatch = await _liveMatchPersistenceService.TryGetLiveMatch(matchId) ?? throw new LiveMatchException(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails, HttpStatusCode.BadRequest);
            foundMatch.MatchStatus = LiveMatchStatus.Finished;
            foundMatch.EndsAt = DateTime.UtcNow;
            await _liveMatchPersistenceService.SetLiveMatch(foundMatch);
            if (shouldUpdateClients)
            {
                await _liveMatchHubContextServiceProvider.UpdateMatchForClients(matchId);
            }
            return foundMatch;
        }
    }
}