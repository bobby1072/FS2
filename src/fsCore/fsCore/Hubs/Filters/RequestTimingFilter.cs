using BT.Common.OperationTimer.Proto;
using Microsoft.AspNetCore.SignalR;

namespace fsCore.Hubs.Filters
{
    public class RequestTimingFilter : IHubFilter
    {
        private readonly ILogger<RequestTimingFilter> _logger;
        public RequestTimingFilter(ILogger<RequestTimingFilter> logger)
        {
            _logger = logger;
        }
        public async ValueTask<object?> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
        {
            var (timeToComplete, valTask) = await OperationTimerUtils.TimeWithResultsAsync(next.Invoke, invocationContext);
            _logger.LogInformation("Hub method {InvocationContext} took {TimeToComplete}ms to complete.", invocationContext.HubMethodName, timeToComplete.Milliseconds);
            return valTask;
        }
    }
}