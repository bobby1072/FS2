using System.Net;
using Common.Misc;
using Common.Models;
using FluentValidation;
using fsCore.ApiModels;
using fsCore.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Services.Abstract;
namespace fsCore.Hubs
{
    [Authorize]
    [RequiredUser]
    [RequiredUserWithGroupPermissions]
    [RequiredSignalRUserConnectionId]
    public class LiveMatchHub : BaseHub
    {
        public const string UpdateMatchMessage = "UpdateMatch";
        public const string CreateMatchMessage = "CreateMatch";
        public const string AllMatchesForUserMessage = "AllMatchesForUser";
        public const string LiveMatchGroupMessage = "LiveMatchGroup";
        public const string ErrorMessage = "Error";
        private readonly ILiveMatchService _liveMatchService;
        private readonly ILiveMatchPersistenceService _liveMatchPersistenceService;
        private readonly ILogger<LiveMatchHub> _logger;
        public LiveMatchHub(ICachingService cachingService, ILiveMatchService liveMatchService, ILiveMatchPersistenceService liveMatchPersistenceService, ILogger<LiveMatchHub> logger) : base(cachingService)
        {
            _liveMatchService = liveMatchService;
            _liveMatchPersistenceService = liveMatchPersistenceService;
            _logger = logger;
        }
        public async Task SaveParticipant(SaveParticipantLiveMatchHubInput hubInput)
        {

        }
        public async Task SaveParticipantLogic(Guid userId, Guid matchId)
        {
            var user = await GetCurrentUserWithPermissionsAsync();

            await _liveMatchService.SaveParticipant(matchId, userId, user);

            var match = await _liveMatchPersistenceService.TryGetLiveMatch(matchId) ?? throw new ApiException(LiveMatchConstants.LiveMatchDoesntExist, HttpStatusCode.NotFound);

            match.RemoveSensitive();

            await Clients.Group($"{LiveMatchGroupMessage}{matchId}").SendAsync(UpdateMatchMessage, HubResponseBuilder.FromLiveMatch(match));
        }
        public async Task CreateMatch(SaveMatchLiveHubInput liveMatch)
        {
            Task methodFunction() => CreateMatchLogic(liveMatch);
            await HubMethodExceptionWrapper((methodFunction, nameof(CreateMatch)));
        }
        private async Task CreateMatchLogic(SaveMatchLiveHubInput liveMatchInput)
        {
            var user = await GetCurrentUserWithPermissionsAsync();

            var liveMatch = liveMatchInput.ToLiveMatch((Guid)user.Id!);
            var createdMatch = await _liveMatchService.CreateMatch(liveMatch, user);

            await Groups.AddToGroupAsync(Context.ConnectionId, $"{LiveMatchGroupMessage}{createdMatch.Id}");

            createdMatch.RemoveSensitive();

            await Clients.Group($"{LiveMatchGroupMessage}{createdMatch.Id}").SendAsync(CreateMatchMessage, HubResponseBuilder.FromLiveMatch(createdMatch));
        }
        public override async Task OnConnectedAsync()
        {
            await HubMethodExceptionWrapper((OnConnectedAsyncLogic, nameof(OnConnectedAsync)));
        }
        private async Task OnConnectedAsyncLogic()
        {
            var user = await GetCurrentUserWithPermissionsAsync();

            var allMatchesForUser = await _liveMatchService.AllMatchesParticipatedIn(user);

            var allMatchesJob = _liveMatchService.AllMatchesParticipatedIn(allMatchesForUser);

            await Task.WhenAll(AddUsersToMatchGroup(allMatchesForUser, Context.ConnectionId),
                allMatchesJob);

            var allMatches = await allMatchesJob;
            var toUpdateParticipants = new List<(LiveMatchParticipant LiveMatchParticipant, Guid MatchId)>();
            foreach (var match in allMatches)
            {
                var myParticipant = match.Participants.FirstOrDefault(x => x.Id == user.Id)!;
                myParticipant.Online = true;
                toUpdateParticipants.Add((myParticipant, match.Id));
            }
            if (allMatches.Count > 0)
            {
                var jobList = new List<Task>
                {
                    _liveMatchPersistenceService.SaveParticipant(toUpdateParticipants)
                };
                foreach (var match in allMatches)
                {
                    match.RemoveSensitive();
                    jobList.Add(Clients.Group($"{LiveMatchGroupMessage}{match.Id}").SendAsync(UpdateMatchMessage, HubResponseBuilder.FromLiveMatch(match)));
                }
                await Task.WhenAll(
                    jobList
                );
            }
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await HubMethodExceptionWrapper((() => OnDisconnectedAsyncLogic(exception), nameof(OnDisconnectedAsync)));
        }
        private async Task OnDisconnectedAsyncLogic(Exception? exception)
        {
            if (exception is not null)
            {
                _logger.LogError(exception, "Signal R connection {ConnectionId} disconnected with exception message {Exception}", Context.ConnectionId, exception.Message);
            }

            var user = await GetCurrentUserWithPermissionsAsync();

            var allMatchesForUser = await _liveMatchService.AllMatchesParticipatedIn(user);

            var allMatches = await _liveMatchService.AllMatchesParticipatedIn(allMatchesForUser);

            var toUpdateParticipants = new List<(LiveMatchParticipant LiveMatchParticipant, Guid MatchId)>();
            foreach (var match in allMatches)
            {
                var myParticipant = match.Participants.FirstOrDefault(x => x.Id == user.Id)!;
                myParticipant.Online = false;
                toUpdateParticipants.Add((myParticipant, match.Id));
            }
            if (allMatches.Count > 0)
            {
                var jobList = new List<Task>
                {
                    _liveMatchPersistenceService.SaveParticipant(toUpdateParticipants)
                };
                foreach (var match in allMatches)
                {
                    match.RemoveSensitive();
                    jobList.Add(Clients.Group($"{LiveMatchGroupMessage}{match.Id}").SendAsync(UpdateMatchMessage, HubResponseBuilder.FromLiveMatch(match)));
                }
                await Task.WhenAll(
                    jobList
                );
            }
        }
        private async Task HubMethodExceptionWrapper((Func<Task> Func, string HubMethodName) hubAction)
        {
            try
            {
                _logger.LogInformation("LiveMatch signal R method: {MethodName} started executing for connection {ConnectionId}", hubAction.HubMethodName, Context.ConnectionId);
                await hubAction.Func.Invoke();
            }
            catch (ApiException e)
            {
                await Clients.Caller.SendAsync(ErrorMessage, HubResponseBuilder.FromError(e));
                _logger.LogError(e, "Signal R method: {MethodName} for connection {ConnectionId} failed with exception message {Exception} and status code {StatusCode}", hubAction.HubMethodName, Context.ConnectionId, e.Message, e.StatusCode);
            }
            catch (ValidationException e)
            {
                await Clients.Caller.SendAsync(ErrorMessage, HubResponseBuilder.FromError(e));
                _logger.LogError(e, "Signal R method: {MethodName} for connection {ConnectionId} failed with exception message {Exception}", hubAction.HubMethodName, Context.ConnectionId, e.Message);
            }
            catch (Exception e)
            {
                await Clients.Caller.SendAsync(ErrorMessage, HubResponseBuilder.FromError(e));
                _logger.LogError(e, "Signal R method: {MethodName} for connection {ConnectionId} failed with exception message {Exception}", hubAction.HubMethodName, Context.ConnectionId, e.Message);
            }
            finally
            {
                _logger.LogInformation("LiveMatch signal R method: {MethodName} finished executing for connection {ConnectionId}", hubAction.HubMethodName, Context.ConnectionId);
            }
        }
        private async Task AddUsersToMatchGroup(ICollection<Guid> matchIds, string connectionId)
        {
            var jobList = new List<Task>();
            foreach (var matchId in matchIds)
            {
                jobList.Add(Groups.AddToGroupAsync(connectionId, $"{LiveMatchGroupMessage}{matchId.ToString()}"));
            }
            await Task.WhenAll(jobList);
        }
    }
}