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