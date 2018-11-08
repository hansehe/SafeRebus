using System;
using System.Globalization;

namespace SafeStandard.Headers
{
    public static class SafeHeaderDateTimeExtensions
    {
        public static string ToSafeHeaderValidString(this DateTime dateTime)
        {
            return dateTime.ToString(CultureInfo.InvariantCulture);
        }
        
        public static DateTime ToDatetimeFromSafeHeaderValidString(this string dateTimeString)
        {
            return DateTime.Parse(dateTimeString, CultureInfo.InvariantCulture);
        }
    }
}