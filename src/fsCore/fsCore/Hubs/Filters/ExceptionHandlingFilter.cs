using System.Net;
using System.Net.Mime;
using System.Text;
using Common;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using Npgsql;

namespace fsCore.Hubs.Filters
{
    internal class ExceptionHandlingFilter : IHubFilter
    {
        private readonly ILogger<ExceptionHandlingFilter> _logger;
        public ExceptionHandlingFilter(ILogger<ExceptionHandlingFilter> logger)
        {
            _logger = logger;
        }
        public async ValueTask<object> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object>> next)
        {
            try
            {
                var httpContext = invocationContext.Context.GetHttpContext() ?? throw new InvalidOperationException("No context");
                try
                {
                    return await next.Invoke(invocationContext);
                }
                catch (ApiException apiException)
                {
                    LogError(apiException.Message, httpContext);
                }
                catch (ValidationException validationException)
                {
                    LogError(CreateValidationExceptionMessage(validationException) ?? ErrorConstants.BadRequest, httpContext);
                }
                catch (NpgsqlException)
                {
                    LogError(ErrorConstants.FailedToPersistData, httpContext);
                }
                catch (Exception)
                {
                    LogError(ErrorConstants.InternalServerError, httpContext);
                }
            }
            catch { }
        }
        private void LogError(string message, HttpContext httpContext)
        {
            _logger.LogError("Request {Request} failed with {Exception}", httpContext.Request.Path, message);
            _logger.LogError("Request from {webToken}", httpContext.Request.Headers.Authorization.ToString());
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