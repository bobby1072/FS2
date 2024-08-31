using System.Text.Json;
using fsCore.Hubs.Filters.Abstract;
using Microsoft.AspNetCore.SignalR;

namespace fsCore.Hubs.Filters.Concrete
{
    public class ExceptionHandlingFilter : IExceptionHandlingFilter
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
                return await next(invocationContext);
            }
            catch (Exception e)
            {
                _logger.LogError("Signal R {Connection} failed with {Exception}", invocationContext.Context.ConnectionId, JsonSerializer.Serialize(e));
                throw;
            }
        }
    }
}