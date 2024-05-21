namespace Common
{
    public static class HangfireConstants
    {
        public static class Queues
        {
            public const string Default = "default";
            public const string StartUpJobs = "startupjobs";
            public static string[] FullList { get; } = new[] { Default, StartUpJobs };
        }
    }
}