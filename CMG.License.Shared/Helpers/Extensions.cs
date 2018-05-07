using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace CMG.License.Shared.Helpers
{
    public static class Extensions
    {
        public static bool ExtractResourceFile(this string resourceFileName, string filePath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources.{resourceFileName}"))
            {
                using (var fileStream = File.Create(filePath + $@"\{resourceFileName}"))
                {
                    if (stream != null)
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        stream.CopyTo(fileStream);
                        return true;
                    }
                    return false;
                };
            }
        }

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