using fsCore.Services.Abstract;
using fsCore.Services.Concrete;
using Microsoft.Extensions.DependencyInjection;

namespace fsCore.Services
{
    public static class BusinessServiceExtensions
    {
        public static IServiceCollection AddBusinessServiceExtensions(
            this IServiceCollection serviceCollection
        )
        {
            serviceCollection.AddHttpClient<IUserInfoClient, UserInfoClient>();

            serviceCollection
                .AddScoped<IWorldFishService, WorldFishService>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<IGroupService, GroupService>()
                .AddScoped<IGroupCatchService, GroupCatchService>()
                .AddScoped<IHangfireJobsService, HangfireJobService>()
                .AddScoped<ICachingService, DistributedCachingService>()
                .AddScoped<ILiveMatchPersistenceService, LiveMatchPersistenceService>()
                .AddScoped<ILiveMatchService, LiveMatchService>();

            return serviceCollection;
        }
    }
}
