using CMG.License.Shared.DataTypes;
using System;
using System.Linq;

namespace CMG.License.Services.Impls
{
    internal static class CheckInTimeProcessingService
    {
        public static DateTime GetCheckInTime(CheckOutDto checkOut, LogFile logFile)
        {
            if (logFile.CheckIns.Count > 0)
            {
                var checkIn = logFile.CheckIns.FirstOrDefault(x => 
                                                    x.ServerHandle == checkOut.ServerHandle
                                                    && x.Product== checkOut.Product
                                                    && x.TimeStamp>checkOut.TimeStamp);
                if (checkIn.TimeStamp != default(DateTime))
                    return checkIn.TimeStamp;
            }
            return default(DateTime);
        }

        public static DateTime GetCheckInTime(LogRptDto checkOutDto,string serverHandle, LogFile logFile)
        {
            if (logFile.CheckIns.Count > 0)
            {
                var checkIn = logFile.CheckIns.FirstOrDefault(x =>
                                                    x.ServerHandle == serverHandle
                                                    && x.Product == checkOutDto.Product);
                if (checkIn.TimeStamp != default(DateTime))
                    return checkIn.TimeStamp;
            }
            return default(DateTime);
        }
    }
}
