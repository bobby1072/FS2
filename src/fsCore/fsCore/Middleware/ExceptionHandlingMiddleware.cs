using System.Net;
using System.Net.Mime;
using System.Text;
using Common;
using FluentValidation;
using Persistence;
namespace fsCore.Middleware
{
    internal class ExceptionHandlingMiddleware : BaseMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger) : base(next)
        {
            _logger = logger;
        }
        private static string CreateValidationExceptionMessage(ValidationException validationException)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < validationException.Errors.Count(); i++)
            {
                var error = validationException.Errors.ElementAt(i);
                sb.Append($"{error.ErrorMessage}. ");
            }
            return sb.ToString();
        }
        private async Task RouteErrorHandler<T>(T error, HttpContext httpContext) where T : Exception
        {
            try
            {

                _logger.LogError(error, error.Message);
                httpContext.Response.Clear();
                httpContext.Response.ContentType = MediaTypeNames.Text.Plain;
                if (error is ApiException apiException)
                {
                    httpContext.Response.StatusCode = (int)apiException.StatusCode;
                    await httpContext.Response.WriteAsync(apiException.Message);
                }
                else if (error is ValidationException validationException)
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await httpContext.Response.WriteAsync(CreateValidationExceptionMessage(validationException) ?? ErrorConstants.BadRequest);
                }
                else
                {
                    var foundPostgresExceptionResults = NpgExceptionHandler.HandleException(error);
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
            catch (Exception) { }
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception e)
            {
                await RouteErrorHandler(e, httpContext);
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