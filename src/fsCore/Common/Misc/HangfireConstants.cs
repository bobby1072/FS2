namespace Common.Misc
{
    public static class HangfireConstants
    {
        public static class Queues
        {
            public const string DefaultJobs = "DefaultJobs";
            public const string StartUpJobs = "StartUpJobs";
            public const string LiveMatchJobs = "LiveMatchJobs";
            public static string[] FullList { get; } = [DefaultJobs, StartUpJobs, LiveMatchJobs];
        }
    }
}