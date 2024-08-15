using System.Net;

namespace Common
{
    public class LiveMatchException : ApiException
    {
        public LiveMatchException(string message, HttpStatusCode statusCode) : base(message, statusCode)
        {
        }
    }
}