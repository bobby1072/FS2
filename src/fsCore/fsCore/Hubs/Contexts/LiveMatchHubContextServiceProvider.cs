using Common.Misc.Abstract;
using Microsoft.AspNetCore.SignalR;
using Services.Abstract;

namespace fsCore.Hubs.Contexts
{
    public class LiveMatchHubContextServiceProvider : ILiveMatchHubContextServiceProvider
    {
        private readonly IHubContext<LiveMatchHub> _hubContext;
        private readonly ILiveMatchPersistenceService _liveMatchPersistenceService;
        private readonly ILogger<LiveMatchHubContextServiceProvider> _logger;
        public LiveMatchHubContextServiceProvider(IHubContext<LiveMatchHub> liveMatchHub, ILiveMatchPersistenceService liveMatchPersistenceService, ILogger<LiveMatchHubContextServiceProvider> logger)
        {
            _hubContext = liveMatchHub;
            _liveMatchPersistenceService = liveMatchPersistenceService;
            _logger = logger;
        }
        public async Task UpdateMatchForClients(Guid matchId)
        {
            try
            {
                var foundMatch = await _liveMatchPersistenceService.TryGetLiveMatch(matchId);
                await _hubContext.Clients.Groups(matchId.ToString()).SendAsync(LiveMatchHub.UpdateMatchMessage, foundMatch);
            }
            catch (Exception e)
            {
                _logger.LogError("Signal R connection failed with {Exception}", e.Message);
            }
        }
    }
}