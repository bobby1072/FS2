using Common.Misc.Abstract;
using fsCore.Hubs.Contexts;
using fsCore.Hubs.Filters.Abstract;
using fsCore.Hubs.Filters.Concrete;
using Microsoft.AspNetCore.SignalR;

namespace fsCore.Hubs
{
    public static class SignalRServiceCollectionExtensions
    {
        public static IServiceCollection AddSignalRFsCore(this IServiceCollection services)
        {
            // services
            //     .AddScoped<IExceptionHandlingFilter, ExceptionHandlingFilter>()
            //     .AddScoped<IUserSessionFilter, UserSessionFilter>()
            //     .AddScoped<IUserWithPermissionsSessionFilter, UserWithPermissionsSessionFilter>()
            //     .AddScoped<IRequiredSignalRUserConnectionIdFilter, RequiredSignalRUserConnectionIdFilter>();

            services.AddScoped<ILiveMatchHubContextServiceProvider, LiveMatchHubContextServiceProvider>();

            services.AddSignalR(opts =>
            {
                opts.AddFilter<ExceptionHandlingFilter>();
                opts.AddFilter<UserSessionFilter>();
                opts.AddFilter<UserWithPermissionsSessionFilter>();
                opts.AddFilter<RequiredSignalRUserConnectionIdFilter>();
            });

            return services;
        }
    }
}