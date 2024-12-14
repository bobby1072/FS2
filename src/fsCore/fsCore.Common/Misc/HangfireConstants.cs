namespace fsCore.Common.Misc
{
    public static class HangfireConstants
    {
        public static class Queues
        {
            public const string DefaultJobs = "default_jobs";
            public const string StartUpJobs = "startup_jobs";
            public const string LiveMatchJobs = "live_match_jobs";
            public static string[] FullList { get; } = [DefaultJobs, StartUpJobs, LiveMatchJobs];
        }
    }
}