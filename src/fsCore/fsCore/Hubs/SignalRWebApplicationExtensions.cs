namespace fsCore.Hubs
{
    public static class SignalRWebApplicationExtensions
    {
        public static WebApplication MapFsCoreHubs(this WebApplication builder)
        {
            builder.MapHub<LiveMatchHub>("Api/SignalR/LiveMatchHub");
            return builder;
        }
    }
}