using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TurnipBot.Services
{
    public static class DateTimeOffsetter
    {
        /// <summary>
        /// Converts a time to the time in a particular time zone.
        /// </summary>
        /// <param name="dateTimeOffset">The date and time to convert.</param>
        /// <param name="destinationTimeZone">The time zone to convert  to.</param>
        /// <returns>The date and time in the destination time zone.</returns>
        public static DateTimeOffset ConvertTime(this DateTimeOffset dateTimeOffset, TimeZoneInfo destinationTimeZone)
        {
            return TimeZoneInfo.ConvertTime(dateTimeOffset, destinationTimeZone);
        }

        /// <summary>
        /// Converts a time to the time in a particular time zone.
        /// </summary>
        /// <param name="dateTimeOffset">The date and time to convert.</param>
        /// <param name="destinationTimeZone">The time zone to convert  to.</param>
        /// <returns>The date and time in the destination time zone.</returns>
        public static DateTimeOffset ConvertTime(this DateTimeOffset dateTimeOffset, string destinationTimeZone)
        {
            return TimeZoneInfo.ConvertTime(dateTimeOffset, TimeZoneInfo.FindSystemTimeZoneById(destinationTimeZone));
        }

        /// <summary>
        /// Converts a time to the time in the Central time zone.
        /// </summary>
        /// <param name="dateTimeOffset">The date and time to convert.</param>
        /// <returns>The date and time in the destination time zone.</returns>
        public static DateTimeOffset ToUSCentralTime(this DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.ConvertTime(GetUSCentralTimeZoneInfo());
        }

        /// <summary>
        /// Windows uses the system registry to fetch time zone information.
        /// Linux instead has the trusty old tz database, which names time zones differently.
        /// I don't recall how it is named on a Mac. We shouldn't be deploying to a Mac docker host anyway. Panic if we do.
        /// </summary>
        /// <returns>TimeZoneInfo for Central Time</returns>
        private static TimeZoneInfo GetUSCentralTimeZoneInfo()
        {
            TimeZoneInfo centralStandardTime = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                centralStandardTime = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                centralStandardTime = TimeZoneInfo.FindSystemTimeZoneById("America/Chicago");
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                throw new NotImplementedException("I don't know how to do a timezone lookup on a Mac.");
            }
            return centralStandardTime;
        }
    }
}
