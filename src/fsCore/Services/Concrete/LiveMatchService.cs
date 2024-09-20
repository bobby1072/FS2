using Common.Misc;
using Common.Misc.Abstract;
using Common.Models;
using Common.Permissions;
using FluentValidation;
using Hangfire;
using Persistence.EntityFramework.Repository.Abstract;
using Services.Abstract;
using System.Net;

namespace Services.Concrete
{
    public class LiveMatchService : ILiveMatchService
    {
        private readonly IActiveLiveMatchParticipantRepository _liveMatchParticipantRepository;
        private readonly IActiveLiveMatchRepository _liveMatchRepository;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILiveMatchHubContextServiceProvider _liveMatchHubContextServiceProvider;
        private readonly ILiveMatchPersistenceService _liveMatchPersistenceService;
        private readonly IValidator<LiveMatch> _liveMatchValidator;
        private readonly IValidator<LiveMatchCatch> _liveMatchCatchValidator;
        private readonly IGroupService _groupService;
        public LiveMatchService(IBackgroundJobClient backgroundJobClient,
            ILiveMatchHubContextServiceProvider liveMatchHubContextServiceProvider,
            ILiveMatchPersistenceService liveMatchPersistenceService,
            IActiveLiveMatchParticipantRepository liveMatchParticipantRepository,
            IActiveLiveMatchRepository liveMatchRepository,
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
            _liveMatchParticipantRepository = liveMatchParticipantRepository;
            _liveMatchRepository = liveMatchRepository;
        }
        public async Task<ICollection<LiveMatch>> AllMatchesParticipatedIn(ICollection<Guid> matchIds)
        {
            var matches = await _liveMatchRepository.GetFullOneById(matchIds);

            return matches ?? throw new LiveMatchException(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails, HttpStatusCode.BadRequest);
        }
        public async Task<ICollection<Guid>> AllMatchesParticipatedIn(UserWithGroupPermissionSet currentUser)
        {
            return await _liveMatchParticipantRepository.GetMatchIdsForUser((Guid)currentUser.Id!);
        }
        public async Task CreateParticipant(Guid matchId, Guid userId, UserWithGroupPermissionSet currentUser)
        {
            var foundMatch = await _liveMatchPersistenceService.TryGetLiveMatch(matchId) ?? throw new LiveMatchException(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails, HttpStatusCode.BadRequest);
            if (!currentUser.GroupPermissions.Can(PermissionConstants.Manage, foundMatch.GroupId) || currentUser.Id != foundMatch.MatchLeaderId)
            {
                throw new LiveMatchException(ErrorConstants.DontHavePermission, HttpStatusCode.Unauthorized);
            }
            if (foundMatch.Participants.Any(x => x.Id == userId))
            {
                return;
            }
            var areUsersInGroup = await _groupService.IsUserInGroup(foundMatch.GroupId, [userId]);

            if (!areUsersInGroup.AllUsersInGroup)
            {
                throw new LiveMatchException(ErrorConstants.DontHavePermission, HttpStatusCode.Unauthorized);
            }

            var user = areUsersInGroup.ActualUsers.FirstOrDefault()!;

            await _liveMatchPersistenceService.SaveParticipant(matchId, LiveMatchParticipant.FromUser(user)!);
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

            match.Participants = [LiveMatchParticipant.FromUser(currentUser, true)!];


            await _liveMatchValidator.ValidateAndThrowAsync(match);
            await _liveMatchPersistenceService.SetLiveMatch(match);
            await _liveMatchPersistenceService.SaveParticipant(match.Id, match.Participants);

            if (match.CommencesAt is not null)
            {
                _backgroundJobClient.Schedule(HangfireConstants.Queues.LiveMatchJobs, () => AutomaticStartMatch(match.Id, currentUser), match.TimeUntilStart!.Value);
            }
            if (match.EndsAt is not null)
            {
                _backgroundJobClient.Schedule(HangfireConstants.Queues.LiveMatchJobs, () => AutomaticEndMatch(match.Id, currentUser), match.TimeUntilEnd!.Value);
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
        public async Task SaveCatch(Guid matchId, LiveMatchCatch liveMatchCatch, UserWithGroupPermissionSet currentUser)
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
                    throw new LiveMatchException(ErrorConstants.NotAllowedToEditThoseFields, HttpStatusCode.Unauthorized);
                }
            }
            liveMatchCatch.CountsInMatch = isCatchValid;
            if (liveMatchCatch.Id is null)
            {
                liveMatchCatch.ApplyDefaults();
                await _liveMatchPersistenceService.SaveCatch(foundMatch.Id, liveMatchCatch);
            }
            else
            {
                await _liveMatchPersistenceService.SaveCatch(foundMatch.Id, liveMatchCatch);
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
        [AutomaticRetry(Attempts = 3, LogEvents = true, DelaysInSeconds = [1], OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public async Task AutomaticStartMatch(Guid matchId, UserWithGroupPermissionSet userWithGroupPermissionSet)
        {
            await StartMatch(matchId, userWithGroupPermissionSet);
            await _liveMatchHubContextServiceProvider.UpdateMatchForClients(matchId);
        }
        [AutomaticRetry(Attempts = 3, LogEvents = true, DelaysInSeconds = [1], OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public async Task AutomaticEndMatch(Guid matchId, UserWithGroupPermissionSet userWithGroupPermissionSet)
        {
            await EndMatch(matchId, userWithGroupPermissionSet);
            await _liveMatchHubContextServiceProvider.UpdateMatchForClients(matchId);
        }
    }
}