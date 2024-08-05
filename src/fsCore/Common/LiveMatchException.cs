namespace Common
{
    public class LiveMatchException : Exception
    {
        public LiveMatchException(string message) : base(message)
        {
        }
        public LiveMatchException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}