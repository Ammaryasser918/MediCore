using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediCore.Application.Common.Extentions
{
    public static class DateTimeExtensions
    {
        public static DateTime? ToEgyptTime(this DateTime? utcDateTime)
        {
            if (!utcDateTime.HasValue) return null;

            var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime.Value, egyptTimeZone);
        }

        public static DateTime ToEgyptTime(this DateTime utcDateTime)
        {
            var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, egyptTimeZone);
        }
    }
}
