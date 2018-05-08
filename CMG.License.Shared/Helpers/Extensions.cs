using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace CMG.License.Shared.Helpers
{
    public static class Extensions
    {
        public static DateTime GetFormattedDateTime(this string dateTimeStr)
        {
            DateTime date = DateTime.MinValue;
            if (!string.IsNullOrEmpty(dateTimeStr))
                if (!DateTime.TryParseExact(dateTimeStr,
                                "yyyy/MM/dd HH:mm:ss",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.None,
                                out date))
                {
                    DateTime.TryParseExact(dateTimeStr,
                                   "yyyy/MM/dd HH:mm",
                                   CultureInfo.InvariantCulture,
                                   DateTimeStyles.None,
                                   out date);
                }
            return date;
        }

    }
}