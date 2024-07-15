using System.Net;
namespace Common
{
    public class ApiException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public ApiException(string message = ErrorConstants.InternalServerError, HttpStatusCode status = HttpStatusCode.InternalServerError) : base(message)
        {
            StatusCode = status;
        }
        public ApiException(Exception innerException, string message = ErrorConstants.InternalServerError, HttpStatusCode status = HttpStatusCode.InternalServerError) : base(message, innerException)
        {
            StatusCode = status;
        }
    }
}