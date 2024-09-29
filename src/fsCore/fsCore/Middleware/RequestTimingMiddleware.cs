using BT.Common.OperationTimer.Proto;

namespace fsCore.Middleware
{
    internal class RequestTimingMiddleware : BaseMiddleware
    {
        private readonly ILogger<RequestTimingMiddleware> _logger;
        public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger) : base(next)
        {
            _logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            var completedOperation = await OperationTimerUtils.TimeAsync(_next.Invoke, context);
            _logger.LogInformation("Request completed in {time}ms", completedOperation.Milliseconds);
        }
    }
}