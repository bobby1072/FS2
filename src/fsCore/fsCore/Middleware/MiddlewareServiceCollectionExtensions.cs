namespace fsCore.Middleware
{
    internal static class MiddlewareServiceCollectionExtensions
    {
        public static IApplicationBuilder UseDefaultMiddlewares(
            this IApplicationBuilder builder)
        {
            return builder
                .UseMiddleware<ExceptionHandlingMiddleware>()
                .UseMiddleware<UserSessionMiddleware>();
        }
    }
}