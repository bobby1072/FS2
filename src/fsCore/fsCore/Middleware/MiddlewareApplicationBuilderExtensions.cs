namespace fsCore.Middleware
{
    internal static class MiddlewareServiceCollectionExtensions
    {
        public static IApplicationBuilder MiddlewareApplicationBuilderExtensions(
            this IApplicationBuilder builder)
        {
            return builder
                .UseMiddleware<ExceptionHandlingMiddleware>()
                .UseMiddleware<UserSessionMiddleware>()
                .UseMiddleware<UserWithPermissionsSessionMiddleware>();
        }
    }
}