using fsCore.Services.Abstract;

namespace fsCore.Api.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static async Task<IServiceProvider> RegisterHangfireJobs(
            this IServiceProvider serviceProvider
        )
        {
            await using var scope = serviceProvider.CreateAsyncScope();

            var hangfire = scope.ServiceProvider.GetRequiredService<IHangfireJobsService>();

            hangfire.RegisterJobs();

            return serviceProvider;
        }
    }
}
