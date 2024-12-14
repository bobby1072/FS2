using BT.Common.OperationTimer.Proto;

namespace fsCore.Api.Middleware
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
            _logger.LogInformation("Request for {RequestPath} completed in {Time}ms", context.Request.Path, completedOperation.Milliseconds);
        }
    }
}