using Common.Misc.Abstract;
using fsCore.Hubs.Contexts;
using fsCore.Hubs.Filters;
using Microsoft.AspNetCore.SignalR;

namespace fsCore.Hubs
{
    public static class SignalRServiceCollectionExtensions
    {
        public static IServiceCollection AddSignalRFsCore(this IServiceCollection services)
        {
            services.AddScoped<ILiveMatchHubContextServiceProvider, LiveMatchHubContextServiceProvider>();

            services.AddSignalR(opts =>
            {
                opts.AddFilter<ExceptionHandlingFilter>();
                opts.AddFilter<RequestTimingFilter>();
                opts.AddFilter<UserSessionFilter>();
                opts.AddFilter<UserWithPermissionsSessionFilter>();
                opts.AddFilter<RequiredSignalRUserConnectionIdFilter>();
            });

            return services;
        }
    }
}