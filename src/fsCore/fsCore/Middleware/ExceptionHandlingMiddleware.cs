using System.Net;
using Common;
using FluentValidation;

namespace fsCore.Middleware
{
    internal class ExceptionHandlingMiddleware : BaseMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger) : base(next)
        {
            _logger = logger;
        }
        private async Task _routeErrorHandler<T>(T error, HttpContext httpContext) where T : Exception
        {
            _logger.LogError(error, error.Message);
            httpContext.Response.Clear();
            httpContext.Response.ContentType = "text/plain";
            if (error is ApiException apiException)
            {
                httpContext.Response.StatusCode = (int)apiException.StatusCode;
                await httpContext.Response.WriteAsync(apiException.Message);
            }
            else if (error is ValidationException validationException)
            {
                var mainError = validationException.Errors.FirstOrDefault();
                httpContext.Response.StatusCode = mainError?.ErrorCode is not null && mainError.ErrorCode.All(char.IsNumber) ? int.Parse(mainError.ErrorCode) : (int)HttpStatusCode.BadRequest;
                await httpContext.Response.WriteAsync(mainError?.ErrorMessage ?? ErrorConstants.BadRequest);
            }
            else
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await httpContext.Response.WriteAsync(ErrorConstants.InternalServerError);
            }
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