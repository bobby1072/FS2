using System.Net;
namespace Common
{
    public class ApiException : Exception
    {
        private const string _internalServerError = "Internal Server Error";
        public HttpStatusCode StatusCode { get; set; }
        public ApiException(string message = _internalServerError, HttpStatusCode status = HttpStatusCode.InternalServerError) : base(message)
        {
            StatusCode = status;
        }
    }
}