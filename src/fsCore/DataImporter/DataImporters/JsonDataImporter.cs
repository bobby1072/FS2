using Common;
using Hangfire;

namespace DataImporter.ModelImporters
{
    public class JsonDataImporter : IDataImporter
    {
        [Queue(HangfireConstants.Queues.StartUpJobs)]
        [AutomaticRetry(Attempts = 3, LogEvents = true, DelaysInSeconds = new[] { 10 }, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public Task Import()
        {
            throw new NotImplementedException();
        }
    }
}