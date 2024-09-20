using fsCore.Attributes;
using Microsoft.AspNetCore.Authorization;
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
            await AddUsersToMatchGroup(allMatchesForUser, Context.ConnectionId);

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