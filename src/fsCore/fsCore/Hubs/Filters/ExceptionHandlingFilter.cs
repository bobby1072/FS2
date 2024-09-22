using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace fsCore.Hubs.Filters
{
    public class ExceptionHandlingFilter : IHubFilter
    {
        private readonly ILogger<ExceptionHandlingFilter> _logger;
        public ExceptionHandlingFilter(ILogger<ExceptionHandlingFilter> logger)
        {
            _logger = logger;
        }
        public async ValueTask<object?> InvokeMethodAsync(
                HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
        {
            try
            {
                return await next.Invoke(invocationContext);
            }
            catch (Exception e)
            {
                _logger.LogError("Signal R {Connection} failed with {Exception}", invocationContext.Context.ConnectionId, JsonSerializer.Serialize(e));
                throw;
            }
        }
    }
}