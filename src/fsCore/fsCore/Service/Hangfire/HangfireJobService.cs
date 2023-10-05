using Hangfire;

namespace fsCore.Service.Hangfire
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
        public void RegisterRecurringJobs()
        {
            RegisterHourlyJobs();
            RegisterDailyJobs();
            RegisterWeeklyJobs();
            RegisterMonthlyJobs();
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
            _recurringJobManager.AddOrUpdate<IWorldFishService>("Migrate fish to db", service => service.MigrateJsonFishToDb(), Cron.Monthly());
            _backgroundJobs.Enqueue<IWorldFishService>(service => service.MigrateJsonFishToDb());
        }
    }
}