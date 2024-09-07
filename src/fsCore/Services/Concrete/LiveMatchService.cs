using Common.Misc;
using Common.Misc.Abstract;
using Common.Models;
using Common.Permissions;
using FluentValidation;
using Hangfire;
using Services.Abstract;
using System.Net;

namespace Services.Concrete
{
    public class LiveMatchService : ILiveMatchService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILiveMatchHubContextServiceProvider _liveMatchHubContextServiceProvider;
        private readonly ILiveMatchPersistenceService _liveMatchPersistenceService;
        private readonly IValidator<LiveMatch> _liveMatchValidator;
        private readonly IValidator<LiveMatchCatch> _liveMatchCatchValidator;
        private readonly IGroupService _groupService;
        public LiveMatchService(IBackgroundJobClient backgroundJobClient,
            ILiveMatchHubContextServiceProvider liveMatchHubContextServiceProvider,
            ILiveMatchPersistenceService liveMatchPersistenceService,
            IValidator<LiveMatch> liveMatchValidator,
            IValidator<LiveMatchCatch> liveMatchCatchValidator,
            IGroupService groupService)
        {
            _backgroundJobClient = backgroundJobClient;
            _liveMatchHubContextServiceProvider = liveMatchHubContextServiceProvider;
            _liveMatchPersistenceService = liveMatchPersistenceService;
            _liveMatchCatchValidator = liveMatchCatchValidator;
            _liveMatchValidator = liveMatchValidator;
            _groupService = groupService;
        }
        public async Task CreateParticipant(Guid matchId, Guid participantId, UserWithGroupPermissionSet currentUser)
        {
            var foundMatch = await _liveMatchPersistenceService.TryGetLiveMatch(matchId) ?? throw new LiveMatchException(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails, HttpStatusCode.BadRequest);
            if (foundMatch.Participants.Any(x => x.Id == participantId))
            {
                return;
            }
            if (!currentUser.GroupPermissions.Can(PermissionConstants.Manage, foundMatch.GroupId) || currentUser.Id != foundMatch.MatchLeaderId)
            {
                throw new LiveMatchException(ErrorConstants.DontHavePermission, HttpStatusCode.Unauthorized);
            }
            var areUsersInGroup = await _groupService.IsUserInGroup(foundMatch.GroupId, [participantId]);

            if (!areUsersInGroup.AllUsersInGroup)
            {
                throw new LiveMatchException(ErrorConstants.DontHavePermission, HttpStatusCode.Unauthorized);
            }

            await _liveMatchPersistenceService.SaveParticipant(matchId, areUsersInGroup.ActualUsers.FirstOrDefault()!);
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


            var areUsersInGroup = await _groupService.IsUserInGroup(match.GroupId, match.Participants.Append(currentUser).Select(x => x.Id ?? throw new LiveMatchException(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails, HttpStatusCode.BadRequest)).Distinct().ToList());

            if (!areUsersInGroup.AllUsersInGroup)
            {
                throw new LiveMatchException(ErrorConstants.DontHavePermission, HttpStatusCode.Unauthorized);
            }

            match.Participants = areUsersInGroup.ActualUsers.ToList();

            match.ApplyDefaults(LiveMatchStatus.NotStarted, currentUser);
            await _liveMatchValidator.ValidateAndThrowAsync(match);
            await _liveMatchPersistenceService.SetLiveMatch(match);
            await _liveMatchPersistenceService.SaveParticipant(match.Id, match.Participants);

            if (match.CommencesAt is not null)
            {
                _backgroundJobClient.Schedule(() => AutomaticStartMatch(match.Id, currentUser), match.TimeUntilStart!.Value);
            }
            if (match.EndsAt is not null)
            {
                _backgroundJobClient.Schedule(() => AutomaticEndMatch(match.Id, currentUser), match.TimeUntilEnd!.Value);
            }

            return match;

        }
        public async Task UpdateMatch(LiveMatch match, UserWithGroupPermissionSet currentUser)
        {
            var foundMatch = await _liveMatchPersistenceService.TryGetLiveMatch(match.Id) ?? throw new LiveMatchException(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails, HttpStatusCode.BadRequest);
            match.Catches = [];
            match.Participants = [];
            match.MatchStatus = foundMatch.MatchStatus;
            match.ApplyDefaults(LiveMatchStatus.InProgress, currentUser);
            if (!currentUser.GroupPermissions.Can(PermissionConstants.Manage, foundMatch.GroupId) || currentUser.Id != foundMatch.MatchLeaderId)
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
            liveMatchCatch.CountsInMatch = isCatchValid;
            if (liveMatchCatch.Id is null)
            {
                liveMatchCatch.ApplyDefaults();
                await _liveMatchPersistenceService.SaveCatch(foundMatch.Id, liveMatchCatch);
                return liveMatchCatch;
            }
            else
            {
                await _liveMatchPersistenceService.SaveCatch(foundMatch.Id, liveMatchCatch);
                return liveMatchCatch;
            }
        }
        public async Task<LiveMatch> StartMatch(Guid matchId, UserWithGroupPermissionSet userWithGroupPermissionSet)
        {
            var foundMatch = await _liveMatchPersistenceService.TryGetLiveMatch(matchId) ?? throw new LiveMatchException(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails, HttpStatusCode.BadRequest);
            if (!userWithGroupPermissionSet.GroupPermissions.Can(PermissionConstants.Manage, foundMatch.GroupId) || userWithGroupPermissionSet.Id != foundMatch.MatchLeaderId)
            {
                throw new LiveMatchException(ErrorConstants.DontHavePermission, HttpStatusCode.Unauthorized);
            }
            if (foundMatch.MatchStatus == LiveMatchStatus.InProgress || foundMatch.MatchStatus == LiveMatchStatus.Finished)
            {
                return foundMatch;
            }
            foundMatch.MatchStatus = LiveMatchStatus.InProgress;
            foundMatch.CommencesAt = DateTime.UtcNow;
            await _liveMatchPersistenceService.SetLiveMatch(foundMatch);
            return foundMatch;
        }
        public async Task<LiveMatch> EndMatch(Guid matchId, UserWithGroupPermissionSet userWithGroupPermissionSet)
        {
            var foundMatch = await _liveMatchPersistenceService.TryGetLiveMatch(matchId) ?? throw new LiveMatchException(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails, HttpStatusCode.BadRequest);
            if (!userWithGroupPermissionSet.GroupPermissions.Can(PermissionConstants.Manage, foundMatch.GroupId) || userWithGroupPermissionSet.Id != foundMatch.MatchLeaderId)
            {
                throw new LiveMatchException(ErrorConstants.DontHavePermission, HttpStatusCode.Unauthorized);
            }
            if (foundMatch.MatchStatus == LiveMatchStatus.Finished)
            {
                return foundMatch;
            }
            foundMatch.MatchStatus = LiveMatchStatus.Finished;
            foundMatch.EndsAt = DateTime.UtcNow;
            await _liveMatchPersistenceService.SetLiveMatch(foundMatch);
            return foundMatch;
        }
        [Queue(HangfireConstants.Queues.LiveMatchJobs)]
        [AutomaticRetry(Attempts = 3, LogEvents = true, DelaysInSeconds = [1], OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public async Task AutomaticStartMatch(Guid matchId, UserWithGroupPermissionSet userWithGroupPermissionSet)
        {
            await StartMatch(matchId, userWithGroupPermissionSet);
            await _liveMatchHubContextServiceProvider.UpdateMatchForClients(matchId);
        }
        [Queue(HangfireConstants.Queues.LiveMatchJobs)]
        [AutomaticRetry(Attempts = 3, LogEvents = true, DelaysInSeconds = [1], OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public async Task AutomaticEndMatch(Guid matchId, UserWithGroupPermissionSet userWithGroupPermissionSet)
        {
            await EndMatch(matchId, userWithGroupPermissionSet);
            await _liveMatchHubContextServiceProvider.UpdateMatchForClients(matchId);
        }
    }
}