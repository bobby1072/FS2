namespace fsCore.Api.Middleware
{
    internal static class MiddlewareApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDefaultMiddleware(
            this IApplicationBuilder builder)
        {
            return builder
                .UseMiddleware<ExceptionHandlingMiddleware>()
                .UseMiddleware<RequestTimingMiddleware>()
                .UseMiddleware<UserSessionMiddleware>()
                .UseMiddleware<UserWithPermissionsSessionMiddleware>();
        }
    }
}