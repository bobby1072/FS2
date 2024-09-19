using fsCore.Attributes;
using fsCore.RequestModels;
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
        public const string AllMatchesForUser = "AllMatchesForUser";
        private readonly ILiveMatchService _liveMatchService;
        private readonly ILiveMatchPersistenceService _liveMatchPersistenceService;
        public LiveMatchHub(ICachingService cachingService, ILiveMatchService liveMatchService, ILiveMatchPersistenceService liveMatchPersistenceService) : base(cachingService)
        {
            _liveMatchService = liveMatchService;
            _liveMatchPersistenceService = liveMatchPersistenceService;
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

            await Clients.Caller.SendAsync(AllMatchesForUser, HubResponse.FromLiveMatch(allMatches));
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
        private async Task AddUsersToMatchGroup(Guid matchId, ICollection<string> connectionIds)
        {
            var jobList = new List<Task>();
            foreach (var connectionId in connectionIds)
            {
                jobList.Add(Groups.AddToGroupAsync(connectionId, matchId.ToString()));
            }
            await Task.WhenAll(jobList);
        }
    }
}