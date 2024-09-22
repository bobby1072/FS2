using Common.Misc;
using Common.Models;
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
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
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
                    jobList.Add(Clients.Group($"{LiveMatchGroupMessage}{match.Id}").SendAsync(UpdateMatchMessage, HubResponseBuilder.FromLiveMatch(match)));
                }
                await Task.WhenAll(
                    jobList
                );
            }
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
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
                    jobList.Add(Clients.Group($"{LiveMatchGroupMessage}{match.Id}").SendAsync(UpdateMatchMessage, HubResponseBuilder.FromLiveMatch(match)));
                }
                await Task.WhenAll(
                    jobList
                );
            }
        }
        private async Task LiveMatchHubExceptionWrapper(Func<Task> hubAction)
        {
            try
            {
                await hubAction.Invoke();
            }
            catch (LiveMatchException e)
            {
                await Clients.Caller.SendAsync(ErrorMessage, HubResponseBuilder.FromError(e));
                _logger.LogError("Signal R connection failed with {Exception}", e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError("Signal R connection failed with {Exception}", e.Message);
            }
            finally
            {
            }
        }
        private async Task AddUsersToMatchGroup(ICollection<Guid> matchIds, string connectionId)
        {
            var jobList = new List<Task>();
            foreach (var matchId in matchIds)
            {
                jobList.Add(Groups.AddToGroupAsync(connectionId, matchId.ToString()));
            }
            await Task.WhenAll(jobList);
        }
        // private async Task AddUsersToMatchGroup(Guid matchId, ICollection<string> connectionIds)
        // {
        //     var jobList = new List<Task>();
        //     foreach (var connectionId in connectionIds)
        //     {
        //         jobList.Add(Groups.AddToGroupAsync(connectionId, matchId.ToString()));
        //     }
        //     await Task.WhenAll(jobList);
        // }
    }
}