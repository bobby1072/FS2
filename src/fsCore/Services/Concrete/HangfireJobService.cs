using fsCore.Common.Misc;
using fsCore.DataImporter;
using Hangfire;
using Microsoft.Extensions.Logging;
using Services.Abstract;

namespace Services.Concrete
{
    public class HangfireJobService : IHangfireJobsService
    {
        private readonly IBackgroundJobClient _backgroundJobs;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly ILogger<HangfireJobService> _logger;
        public HangfireJobService(
            IBackgroundJobClient backgroundJobs,
            ILogger<HangfireJobService> logger,
            IRecurringJobManager recurringJobManager
            )
        {
            _backgroundJobs = backgroundJobs;
            _logger = logger;
            _recurringJobManager = recurringJobManager;
        }
        public void RegisterJobs()
        {
            RegisterHourlyJobs();
            RegisterDailyJobs();
            RegisterWeeklyJobs();
            RegisterMonthlyJobs();
            RegisterStartupJobs();
        }
        public void RegisterHourlyJobs()
        {

        }
        public void RegisterDailyJobs()
        {

        }
        public void RegisterWeeklyJobs()
        {
        }
        public void RegisterMonthlyJobs()
        {
        }
        public void RegisterStartupJobs()
        {
            _backgroundJobs.Enqueue<IWorldFishService>(HangfireConstants.Queues.StartUpJobs, service => service.MigrateJsonFishToDb());
            _backgroundJobs.Enqueue<IDataImporter>(HangfireConstants.Queues.StartUpJobs, importer => importer.Import());
        }
    }
}