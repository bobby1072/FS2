using System.Net;
using Common;

namespace fsCore.Middleware
{
    internal class ExceptionHandlingMiddleware : BaseMiddleware
    {
        public ExceptionHandlingMiddleware(RequestDelegate next) : base(next) { }
        protected async Task _routeErrorHandler<T>(T error, HttpContext httpContext) where T : Exception
        {
            httpContext.Response.Clear();
            httpContext.Response.ContentType = "text/plain";
            if (error is ApiException apiException)
            {
                httpContext.Response.StatusCode = (int)apiException.StatusCode;
                await httpContext.Response.WriteAsync(apiException.Message);
                return;
            }
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await httpContext.Response.WriteAsync(string.IsNullOrEmpty(error.Message) ? ErrorConstants.InternalServerError : error.Message);
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