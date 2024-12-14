namespace fsCore.Hubs
{
    public static class SignalRWebApplicationExtensions
    {
        public static WebApplication MapFsCoreHubs(this WebApplication builder, bool useLiveMatch)
        {
            if (useLiveMatch)
            {
                builder.MapHub<LiveMatchHub>("Api/SignalR/LiveMatchHub");
                return builder;
            }

            return builder;
        }
    }
}