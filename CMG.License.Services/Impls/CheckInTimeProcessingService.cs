using CMG.License.Shared.DataTypes;
using CMG.License.Shared.Helpers;
using System;
using System.Linq;

namespace CMG.License.Services.Impls
{
    internal static class CheckInTimeProcessingService
    {
        public static DateTime GetCheckInTime(int checkOutKey, LogFile logFile)
        {
            string strCheckInTime = string.Empty;
            var checkOut = logFile.CheckOuts[checkOutKey];
            var OutServerHandle = checkOut[CheckOut.server_handle];
            var OutProduct = checkOut[CheckOut.product];
            if (logFile.CheckIns.Count > 0)
            {
                var checkIn = logFile.CheckIns.FirstOrDefault(x => 
                                                    x.Value[CheckIn.server_handle] == OutServerHandle
                                                    && x.Value[CheckIn.product]== OutProduct
                                                    && x.Key>checkOutKey).Value;
                if (checkIn != null)
                    strCheckInTime = $"{logFile.Year}/{checkIn[CheckIn.mm_dd]} {checkIn[CheckIn.time]}";
            }
            return strCheckInTime.GetFormattedDateTime();
        }

        public static DateTime GetCheckInTime(LogRptDto checkOutDto,string serverHandle, LogFile logFile)
        {
            string strCheckInTime = string.Empty;
            var OutServerHandle = serverHandle;
            var OutProduct = checkOutDto.Product;
            if (logFile.CheckIns.Count > 0)
            {
                var checkIn = logFile.CheckIns.Values.FirstOrDefault(x =>
                                                    x[CheckIn.server_handle] == OutServerHandle
                                                    && x[CheckIn.product] == OutProduct);
                if (checkIn != null)
                    strCheckInTime = $"{logFile.Year}/{checkIn[CheckIn.mm_dd]} {checkIn[CheckIn.time]}";
            }
            return strCheckInTime.GetFormattedDateTime();
        }
    }
}
