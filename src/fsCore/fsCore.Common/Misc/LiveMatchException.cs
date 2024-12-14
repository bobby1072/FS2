using System.Net;

namespace fsCore.Common.Misc
{
    public class LiveMatchException : ApiException
    {
        public LiveMatchException(string message, HttpStatusCode statusCode) : base(message, statusCode)
        {
        }
    }
}