using Common.Misc;
using FluentValidation;
using Npgsql;
using System.Net;
using System.Net.Mime;
using System.Text;
namespace fsCore.Middleware
{
    internal class ExceptionHandlingMiddleware : BaseMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger) : base(next)
        {
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                try
                {
                    await _next.Invoke(httpContext);
                }
                catch (ApiException apiException)
                {
                    await HandleError(apiException.Message, apiException.StatusCode, httpContext);
                }
                catch (ValidationException validationException)
                {
                    await HandleError(CreateValidationExceptionMessage(validationException) ?? ErrorConstants.BadRequest, HttpStatusCode.BadRequest, httpContext);
                }
                catch (NpgsqlException)
                {
                    await HandleError(ErrorConstants.FailedToPersistData, HttpStatusCode.InternalServerError, httpContext);
                }
                catch (Exception)
                {
                    await HandleError(ErrorConstants.InternalServerError, HttpStatusCode.InternalServerError, httpContext);
                }
            }
            catch { }
        }
        private async Task HandleError(string message, HttpStatusCode statusCode, HttpContext httpContext)
        {
            _logger.LogError("Request {Request} failed with {Exception}", httpContext.Request.Path, message);
            _logger.LogError("Request from {webToken}", httpContext.Request.Headers.Authorization.ToString());
            httpContext.Response.Clear();
            httpContext.Response.ContentType = MediaTypeNames.Text.Plain;
            httpContext.Response.StatusCode = (int)statusCode;
            await httpContext.Response.WriteAsync(message);
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