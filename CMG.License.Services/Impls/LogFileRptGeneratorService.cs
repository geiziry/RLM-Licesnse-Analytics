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

            foreach (var checkOutKey in logFile.CheckOuts.Keys)
            {
                var logRptDto = new LogRptDto
                {
                    Product = logFile.CheckOuts[checkOutKey][CheckOut.product],
                    Version = logFile.CheckOuts[checkOutKey][CheckOut.version],
                    UserName = logFile.CheckOuts[checkOutKey][CheckOut.user],
                    HostName = logFile.CheckOuts[checkOutKey][CheckOut.host],
                    InstalledCount = Int32.Parse(GetProductInstalledLicCount(logFile.CheckOuts[checkOutKey][CheckOut.product], logFile)),
                    InUse = Int32.Parse(logFile.CheckOuts[checkOutKey][CheckOut.cur_use]),
                    OutTime = DateTime.ParseExact($"{year}/{logFile.CheckOuts[checkOutKey][CheckOut.mm_dd]} {logFile.CheckOuts[checkOutKey][CheckOut.time]}",
                    "yyyy/MM/dd HH:mm:ss",
                    System.Globalization.CultureInfo.InvariantCulture),
                    InTime = DateTime.ParseExact($"{year}{GetStrCheckInTime(logFile.CheckOuts[checkOutKey][CheckOut.server_handle], logFile)}",
                    "yyyy/MM/dd HH:mm:ss",
                    System.Globalization.CultureInfo.InvariantCulture),
                    RequestTime = DateTime.ParseExact($"{year}{GetStrRequestTime(checkOutKey, logFile)}",
                    "yyyy/MM/dd HH:mm:ss",
                    System.Globalization.CultureInfo.InvariantCulture)
                };

                report.Add(logRptDto);
            }

            return report;
        }

        private object GetStrRequestTime(int checkOutKey, LogFile logFile)
        {
            string strRequestTime = string.Empty;
            if (!logFile.Denials.Any())
                strRequestTime = $"/{logFile.CheckOuts[checkOutKey][CheckOut.mm_dd]} {logFile.CheckOuts[checkOutKey][CheckOut.time]}";
            else
            {
                string productName = logFile.CheckOuts[checkOutKey][CheckOut.product];
                string userName = logFile.CheckOuts[checkOutKey][CheckOut.user];

                var userDenialsForProduct = logFile.Denials.Where(x =>
                                      x.Value[Deny.user] == userName
                                      && x.Value[Deny.product] == productName);
                if (userDenialsForProduct.Any())
                {
                    var lastCheckOutBeforCurrForProd = logFile.CheckOuts.Where(x => x.Key < checkOutKey
                                                    && x.Value[CheckOut.product] == productName)
                                                    .OrderBy(x => x.Key).LastOrDefault();
                    var firstDenialforUserAfterAbove = userDenialsForProduct.FirstOrDefault(x =>
                                      x.Key > lastCheckOutBeforCurrForProd.Key).Value;
                    strRequestTime = $"/{firstDenialforUserAfterAbove?[Deny.mm_dd]} {firstDenialforUserAfterAbove?[Deny.time]}";
                }
            }
            return strRequestTime;
        }

        private string GetProductInstalledLicCount(string productName, LogFile logFile)
        {
            string licCount = string.Empty;
            if (logFile.Products.Count > 0)
            {
                var product = logFile.Products.Values.FirstOrDefault(x => x[Product.name] == productName);
                licCount = product?[Product.count];
            }
            return licCount;
        }

        private string GetStrCheckInTime(string serverHandle, LogFile logFile)
        {
            string strCheckInTime = string.Empty;
            if (logFile.CheckIns.Count > 0)
            {
                var checkIn = logFile.CheckIns.Values.FirstOrDefault(x => x[CheckIn.server_handle] == serverHandle);
                if (checkIn != null)
                    strCheckInTime = $"/{checkIn[CheckIn.mm_dd]} {checkIn[CheckIn.time]}";
            }
            return strCheckInTime;
        }
    }
}