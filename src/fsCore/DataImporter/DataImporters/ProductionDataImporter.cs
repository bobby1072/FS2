using Hangfire;
using Common.Misc;

namespace DataImporter.DataImporters
{
    internal class ProductionDataImporter : IDataImporter
    {
        [Queue(HangfireConstants.Queues.StartUpJobs)]
        [AutomaticRetry(Attempts = 3, LogEvents = true, DelaysInSeconds = [10], OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public Task Import()
        {
            return Task.CompletedTask;
        }
    }
}