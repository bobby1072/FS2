using System.Text.Json;

namespace fsCore.Middleware
{
    internal class RequestResponseLoggingMiddleware : BaseMiddleware
    {
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger) : base(next)
        {
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            _logger.LogInformation("Request entering: {request}", JsonSerializer.Serialize(httpContext.Request));
            if (httpContext.Request.Headers.Authorization.Count > 0)
            {
                _logger.LogInformation("Request webToken from {webToken}", httpContext.Request.Headers.Authorization.ToString());
            }

            await _next.Invoke(httpContext);

            _logger.LogInformation("Request leaving with response: {request}", JsonSerializer.Serialize(httpContext.Response));
        }
    }
}