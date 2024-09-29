namespace fsCore.Middleware
{
    internal static class MiddlewareApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDefaultMiddleware(
            this IApplicationBuilder builder)
        {
            return builder
                .UseMiddleware<ExceptionHandlingMiddleware>()
                .UseMiddleware<RequestTimingMiddleware>()
                .UseMiddleware<RequestResponseLoggingMiddleware>()
                .UseMiddleware<UserSessionMiddleware>()
                .UseMiddleware<UserWithPermissionsSessionMiddleware>();
        }
    }
}