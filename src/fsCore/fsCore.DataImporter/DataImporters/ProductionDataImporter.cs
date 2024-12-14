using fsCore.DataImporter;
using Hangfire;

namespace fsCore.DataImporter.DataImporters
{
    internal class ProductionDataImporter : IDataImporter
    {
        [AutomaticRetry(Attempts = 3, LogEvents = true, DelaysInSeconds = [10], OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public Task Import()
        {
            return Task.CompletedTask;
        }
    }
}