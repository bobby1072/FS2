using fsCore.Common.Misc;
using FluentValidation;
using Npgsql;
using System.Net;
using System.Net.Mime;
using System.Text;

namespace fsCore.Api.Middleware
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
                await _next.Invoke(httpContext);
            }
            catch (ApiException apiException)
            {
                await HandleError(apiException.Message, apiException.StatusCode, httpContext, apiException, true);
            }
            catch (ValidationException validationException)
            {
                await HandleError(CreateValidationExceptionMessage(validationException) ?? ErrorConstants.BadRequest, HttpStatusCode.BadRequest, httpContext, validationException, true);
            }
            catch (NpgsqlException e)
            {
                await HandleError(ErrorConstants.FailedToSaveData, HttpStatusCode.InternalServerError, httpContext, e, true);
            }
            catch (Exception e)
            {
                await HandleError(ErrorConstants.InternalServerError, HttpStatusCode.InternalServerError, httpContext, e);
            }
        }
        private async Task HandleError(string message, HttpStatusCode statusCode, HttpContext httpContext, Exception? actualException = null, bool logAsInformation = false)
        {
            if (logAsInformation)
            {
                _logger.LogInformation(actualException, "Request {Request} failed with message: {Exception}. Request from: {WebToken}", httpContext.Request.Path, message, httpContext.Request.Headers.Authorization.ToString());
            }
            else
            {
                _logger.LogError(actualException, "Request {Request} failed with message: {Exception}. Request from: {WebToken}", httpContext.Request.Path, message, httpContext.Request.Headers.Authorization.ToString());
            }
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

}