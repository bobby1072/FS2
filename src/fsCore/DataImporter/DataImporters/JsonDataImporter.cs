using Hangfire;

namespace DataImporter.DataImporters
{
    public class JsonDataImporter : IDataImporter
    {
        [AutomaticRetry(Attempts = 3, LogEvents = true, DelaysInSeconds = [10], OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public Task Import()
        {
            throw new NotImplementedException();
        }
    }
}