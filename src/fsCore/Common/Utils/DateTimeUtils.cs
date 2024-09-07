namespace Common.Utils
{
    public static class DateTimeUtils
    {
        public static Func<DateTime> RandomPastDate()
        {
            DateTime start = new DateTime(1995, 1, 1);
            Random gen = new Random();
            int range = (DateTime.UtcNow.AddDays(-3) - start).Days;
            return () => start.AddDays(gen.Next(range));
        }
    }
}
