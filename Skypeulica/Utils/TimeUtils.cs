namespace Skypeulica.Utils
{
    internal static class TimeUtils
    {
        public static long GetUnixTimestamp(DateTime date)
        {
            DateTime zero = new DateTime(1970, 1, 1);
            TimeSpan span = date.Subtract(zero);

            return (long)span.TotalMilliseconds;
        }

    }
}