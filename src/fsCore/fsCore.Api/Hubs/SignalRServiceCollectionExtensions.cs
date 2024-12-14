using fsCore.Api.Hubs.Contexts;
using fsCore.Api.Hubs.Filters;
using fsCore.Common.Misc.Abstract;
using Microsoft.AspNetCore.SignalR;

namespace fsCore.Api.Hubs
{
    public static class SignalRServiceCollectionExtensions
    {
        public static IServiceCollection AddSignalRFsCore(this IServiceCollection services)
        {
            services.AddScoped<
                ILiveMatchHubContextServiceProvider,
                LiveMatchHubContextServiceProvider
            >();

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
