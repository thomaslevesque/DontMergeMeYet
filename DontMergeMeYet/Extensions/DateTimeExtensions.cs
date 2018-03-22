using System;

namespace DontMergeMeYet.Models.Github.Webhooks
{
    static class DateTimeExtensions
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static int ToUnixTimeStamp(this DateTime date)
        {
            return (int) (date - UnixEpoch).TotalSeconds;
        }
    }
}