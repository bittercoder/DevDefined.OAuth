using System;

namespace DevDefined.OAuth.Core
{
    public static class DateTimeUtility
    {
        public static long Epoch(this DateTime d)
        {
            return (long) (d.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static DateTime FromEpoch(long epoch)
        {
            var d = new DateTime(1970, 1, 1);
            d = d.AddSeconds(epoch);
            return d.ToLocalTime();
        }
    }
}