namespace fsCore.Service.Hangfire
{
    public interface IHangfireJobsService
    {
        void RegisterRecurringJobs();
        void RegisterStartupJobs();
    }
}
