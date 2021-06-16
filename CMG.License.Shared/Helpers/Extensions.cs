using CMG.License.Shared.DataTypes;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

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

        public static ExcelRangeBase LoadFromCollectionFiltered<T>(this ExcelRangeBase @this, IEnumerable<T> collection) where T : class
        {
            MemberInfo[] membersToInclude = typeof(T)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => !Attribute.IsDefined(p, typeof(EpplusIgnoreAttribute)))
                .ToArray();

            return @this.LoadFromCollection<T>(collection, true,
                OfficeOpenXml.Table.TableStyles.Light13,
                BindingFlags.Instance | BindingFlags.Public,
                membersToInclude);
        }
    }
}