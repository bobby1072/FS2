using System.Net;
using fsCore.ApiModels;
using fsCore.Common.Misc;
using fsCore.Common.Misc.Abstract;
using fsCore.Services.Abstract;
using Microsoft.AspNetCore.SignalR;

namespace fsCore.Hubs.Contexts
{
    public class LiveMatchHubContextServiceProvider : ILiveMatchHubContextServiceProvider
    {
        private readonly IHubContext<LiveMatchHub> _hubContext;
        private readonly ILiveMatchPersistenceService _liveMatchPersistenceService;
        private readonly ILogger<LiveMatchHubContextServiceProvider> _logger;

        public LiveMatchHubContextServiceProvider(
            IHubContext<LiveMatchHub> liveMatchHub,
            ILiveMatchPersistenceService liveMatchPersistenceService,
            ILogger<LiveMatchHubContextServiceProvider> logger
        )
        {
            _hubContext = liveMatchHub;
            _liveMatchPersistenceService = liveMatchPersistenceService;
            _logger = logger;
        }

        public async Task UpdateMatchForClients(Guid matchId)
        {
            await HubContextServiceProviderMethodExceptionWrapper(
                (() => UpdateMatchForClientsLogic(matchId), nameof(UpdateMatchForClients))
            );
        }

        private async Task UpdateMatchForClientsLogic(Guid matchId)
        {
            var foundMatch =
                await _liveMatchPersistenceService.TryGetLiveMatch(matchId)
                ?? throw new LiveMatchException("Match not found", HttpStatusCode.NotFound);
            await _hubContext
                .Clients.Groups($"{LiveMatchHub.LiveMatchGroupPrefix}{matchId}")
                .SendAsync(
                    LiveMatchHub.UpdateMatchMessage,
                    HubResponseBuilder.FromLiveMatch(foundMatch)
                );
        }

        private async Task HubContextServiceProviderMethodExceptionWrapper(
            (Func<Task> Func, string HubMethodName) hubAction
        )
        {
            _logger.LogInformation(
                "LiveMatch signal R method: {MethodName} started executing",
                hubAction.HubMethodName
            );
            try
            {
                await hubAction.Func.Invoke();
            }
            catch (ApiException e)
            {
                _logger.LogError(
                    "Signal R method: {MethodName} failed with exception message {Exception} and status code {StatusCode}",
                    hubAction.HubMethodName,
                    e.Message,
                    e.StatusCode
                );
            }
            catch (Exception e)
            {
                _logger.LogError(
                    "Signal R method: {MethodName} failed with exception message {Exception}",
                    hubAction.HubMethodName,
                    e.Message
                );
            }
            finally
            {
                _logger.LogInformation(
                    "LiveMatch signal R method: {MethodName} finished executing",
                    hubAction.HubMethodName
                );
            }
        }
    }
}
