using System.Net;
using Common.Misc;
using Common.Misc.Abstract;
using fsCore.ApiModels;
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
                var foundMatch = await _liveMatchPersistenceService.TryGetLiveMatch(matchId) ?? throw new LiveMatchException("Match not found", HttpStatusCode.NotFound);
                await _hubContext.Clients.Groups($"{LiveMatchHub.LiveMatchGroupMessage}{matchId}").SendAsync(LiveMatchHub.UpdateMatchMessage, HubResponseBuilder.FromLiveMatch(foundMatch));
            }
            catch (Exception e)
            {
                _logger.LogError("Signal R connection failed with {Exception}", e.Message);
            }
        }
    }
}