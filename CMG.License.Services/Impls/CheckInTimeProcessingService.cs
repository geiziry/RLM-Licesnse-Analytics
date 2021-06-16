using Akka.Util;
using CMG.License.Shared.DataTypes;
using System;
using System.Linq;

namespace CMG.License.Services.Impls
{
    internal static class CheckInTimeProcessingService
    {
        public static DateTime GetCheckInTime(CheckOutDto checkOut, ConcurrentSet<CheckInDto> checkIns)
        {
            if (checkIns.Count > 0)
            {
                var checkIn = checkIns.FirstOrDefault(x => 
                                                    x.ServerHandle == checkOut.ServerHandle
                                                    && x.Product== checkOut.Product
                                                    && x.User==checkOut.User
                                                    && x.TimeStamp>=checkOut.TimeStamp
                                                    && x.Host==checkOut.Host);
                if (checkIn.TimeStamp != default(DateTime))
                    return checkIn.TimeStamp;
            }
            return default(DateTime);
        }

        public static DateTime GetCheckInTime(LogRptDto checkOutDto, LogFile logFile)
        {
            if (logFile.CheckIns.Count > 0)
            {
                var checkIn = logFile.CheckIns.FirstOrDefault(x =>
                                                    x.ServerHandle == checkOutDto.ServerHandle
                                                    && x.Product == checkOutDto.Product
                                                    && x.User == checkOutDto.UserName
                                                    && x.TimeStamp >= checkOutDto.OutTime
                                                    && x.Host == checkOutDto.HostName);
                if (checkIn.TimeStamp != default(DateTime))
                    return checkIn.TimeStamp;
            }
            return default(DateTime);
        }
    }
}
