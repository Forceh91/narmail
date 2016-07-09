using System;

namespace narmail.Models
{
    public static class Utils
    {
        public static DateTime convertUnixTimestampToDateTime(double unixTimestamp)
        {
            // utc starts at 1970
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // add the seconds on and convert it to local time
            dateTime = dateTime.AddSeconds(unixTimestamp).ToLocalTime();

            // send it back
            return dateTime;
        }
    }
}
