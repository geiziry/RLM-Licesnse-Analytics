using CMG.License.Shared.DataTypes;
using CMG.License.Shared.Helpers;
using System;
using System.Linq;

namespace CMG.License.Services.Impls
{
    internal static class CheckInTimeProcessingService
    {
        public static DateTime GetCheckInTime(string serverHandle, LogFile logFile)
        {
            string strCheckInTime = string.Empty;
            if (logFile.CheckIns.Count > 0)
            {
                var checkIn = logFile.CheckIns.Values.FirstOrDefault(x => x[CheckIn.server_handle] == serverHandle);
                if (checkIn != null)
                    strCheckInTime = $"{logFile.Year}/{checkIn[CheckIn.mm_dd]} {checkIn[CheckIn.time]}";
            }
            return strCheckInTime.GetFormattedDateTime();
        }


    }
}
