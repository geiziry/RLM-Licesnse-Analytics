using CMG.License.Services.Interfaces;
using CMG.License.Shared.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CMG.License.Services.Impls
{
    /// <summary>
    /// Ins and Outs to be mapped according to their server_handle
    /// Request time is same as checkout,
    /// unless the last event for this product is deny,
    /// if multiple denys, then the first deny is the request time
    /// <para Product_Last_Out/>
    /// <para Product_Last_Deny/>
    /// </summary>
    public class LogFileRptGeneratorService : ILogFileRptGeneratorService
    {
        public List<LogRptDto> GenerateReport(LogFile logFile)
        {
            var year = logFile.Start[Start.date].Split('/')[2];
            List<LogRptDto> report = new List<LogRptDto>();

            foreach (var checkOut in logFile.CheckOuts)
            {
                var logRptDto = new LogRptDto
                {
                    Product = checkOut[CheckOut.product],
                    Version = checkOut[CheckOut.version],
                    UserName = checkOut[CheckOut.user],
                    HostName = checkOut[CheckOut.host],
                    InstalledCount = Int32.Parse(GetProductInstalledLicCount(checkOut[CheckOut.product],logFile)),
                    InUse = Int32.Parse(checkOut[CheckOut.cur_use]),
                    OutTime = DateTime.ParseExact($"{year}/{checkOut[CheckOut.mm_dd]} {checkOut[CheckOut.time]}", 
                    "yyyy/MM/dd HH:mm:ss",
                    System.Globalization.CultureInfo.InvariantCulture),
                    InTime = DateTime.ParseExact($"{year}{GetStrCheckInTime(checkOut[CheckOut.server_handle],logFile)}",
                    "yyyy/MM/dd HH:mm:ss",
                    System.Globalization.CultureInfo.InvariantCulture),
                    RequestTime= DateTime.ParseExact($"{year}{GetStrRequestTime(checkOut, logFile)}",
                    "yyyy/MM/dd HH:mm:ss",
                    System.Globalization.CultureInfo.InvariantCulture)
                };

                report.Add(logRptDto);
            }

            return report;
        }

        private object GetStrRequestTime(List<string> checkOut, LogFile logFile)
        {
            string strRequestTime = string.Empty;
            if (!logFile.Denials.Any())
                strRequestTime = $"/{checkOut[CheckOut.mm_dd]} {checkOut[CheckOut.time]}";
            else
            {
                var userDenialsForProduct = logFile.Denials.Where(x =>
                                      x[Deny.user] == checkOut[CheckOut.user]
                                      && x[Deny.product] == checkOut[CheckOut.product]);
                if (userDenialsForProduct.Any())
                {
                    var lastCheckOutBeforCurrForProd=logFile.CheckOuts.las
                }
            }
            return strRequestTime;
        }

        private string GetProductInstalledLicCount(string productName, LogFile logFile)
        {
            string licCount = string.Empty;
            if(logFile.Products.Count>0)
            {
                var product = logFile.Products.FirstOrDefault(x => x[Product.name] == productName);
                licCount = product?[Product.count];
            }
            return licCount;
        }

        private string GetStrCheckInTime(string serverHandle, LogFile logFile)
        {
            string strCheckInTime = string.Empty;
            if (logFile.CheckIns.Count>0)
            {
                var checkIn = logFile.CheckIns.FirstOrDefault(x => x[CheckIn.server_handle] == serverHandle);
                if(checkIn!=null)
                    strCheckInTime=$"/{checkIn[CheckIn.mm_dd]} {checkIn[CheckIn.time]}";
            }
            return strCheckInTime;
        }
    }
}