using System;
using System.Globalization;

namespace CMG.License.Shared.Helpers
{
    //"yyyy/MM/dd HH:mm:ss" for Out & In
    //"yyyy/MM/dd HH:mm" for Deny 
    public static class Extensions
    {
        public static DateTime GetFormattedDateTime(this string dateTimeStr,string format)
        {
            DateTime date = DateTime.MinValue;
            if (!string.IsNullOrEmpty(dateTimeStr))
                DateTime.TryParseExact(dateTimeStr,
                                format,
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.None,
                                out date);
            return date;
        }

    }
}