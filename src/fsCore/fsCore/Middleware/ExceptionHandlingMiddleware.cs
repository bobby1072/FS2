using System.Net;
using Common;

namespace fsCore.Middleware
{
    internal class ExceptionHandlingMiddleware : BaseMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger) : base(next)
        {
            _logger = logger;
        }
        protected async Task _routeErrorHandler<T>(T error, HttpContext httpContext) where T : Exception
        {
            _logger.LogError(error, error.Message);
            httpContext.Response.Clear();
            httpContext.Response.ContentType = "text/plain";
            if (error is ApiException apiException)
            {
                httpContext.Response.StatusCode = (int)apiException.StatusCode;
                await httpContext.Response.WriteAsync(apiException.Message);
                return;
            }
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await httpContext.Response.WriteAsync(ErrorConstants.InternalServerError);
            return;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception e)
            {
                await _routeErrorHandler(e, httpContext);
            }
        }
    }
    internal static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }

}