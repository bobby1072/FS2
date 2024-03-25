using System.Net;
using Common;
using Common.Dbinterfaces.ErrorHandlers;
using FluentValidation;

namespace fsCore.Middleware
{
    internal class ExceptionHandlingMiddleware : BaseMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly INpgExceptionHandler _postgresExceptionHandler;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, INpgExceptionHandler postgresExceptionHandler) : base(next)
        {
            _postgresExceptionHandler = postgresExceptionHandler;
            _logger = logger;
        }
        private async Task _routeErrorHandler<T>(T error, HttpContext httpContext) where T : Exception
        {
            try
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
                    var foundPostgresExceptionResults = await _postgresExceptionHandler.HandleException(error);
                    if (foundPostgresExceptionResults is null)
                    {
                        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        await httpContext.Response.WriteAsync(ErrorConstants.InternalServerError);
                        return;
                    }
                    var (statusCode, message) = foundPostgresExceptionResults.Value;
                    httpContext.Response.StatusCode = statusCode;
                    await httpContext.Response.WriteAsync(message);
                }
            }
            catch (Exception _) { }
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