
using System.Diagnostics;

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
            var stopWatch = Stopwatch.StartNew();
            await _next.Invoke(context);
            stopWatch.Stop();
            _logger.LogInformation("Request for {RequestPath} completed in {Time}ms", context.Request.Path, stopWatch.ElapsedMilliseconds);
        }
    }
}