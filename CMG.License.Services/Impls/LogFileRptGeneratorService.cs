using CMG.License.Services.Interfaces;
using CMG.License.Shared.DataTypes;
using CMG.License.Shared.Helpers;
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
        private Dictionary<string, LogRptDto> InUseCheckOuts;
        private List<LogRptDto> report;

        public void InitializeReport()
        {
            InUseCheckOuts = new Dictionary<string, LogRptDto>();
            report = new List<LogRptDto>();
        }

        public void GenerateReport(LogFile logFile)
        {
            if (InUseCheckOuts.Any())
                GetCheckInforInUseOuts(logFile);
            int i = 0;
            foreach (var checkOutKey in logFile.CheckOuts.Keys)
            {
                i++;
                logFile.ProgressInt = i*100 / logFile.CheckOuts.Count;
                var checkOut = logFile.CheckOuts[checkOutKey];
                var product = checkOut[CheckOut.product];
                var server_handle = checkOut[CheckOut.server_handle];
                var logRptDto = new LogRptDto
                {
                    Product = product,
                    Version = checkOut[CheckOut.version],
                    UserName = checkOut[CheckOut.user],
                    HostName = checkOut[CheckOut.host],
                    InstalledCount = Int32.Parse(GetProductInstalledLicCount(product, logFile)),
                    InUse = Int32.Parse(checkOut[CheckOut.cur_use]),
                    OutTime = GetOutTime(checkOutKey, logFile),
                    InTime = CheckInTimeProcessingService.GetCheckInTime(checkOutKey, logFile),
                    RequestTime = RequestTimeProcessingService.GetStrRequestTime(checkOutKey, logFile)
                };
                if (logRptDto.InTime > DateTime.MinValue)
                    report.Add(logRptDto);
                else
                    InUseCheckOuts[server_handle] = logRptDto;
            }
        }

        private void GetCheckInforInUseOuts(LogFile logFile)
        {
            foreach (var server_handle in InUseCheckOuts.Keys.ToList())
            {
                var noCheckInlogRptDto = InUseCheckOuts[server_handle];
                var InTime = CheckInTimeProcessingService.GetCheckInTime(noCheckInlogRptDto,server_handle, logFile);
                if (InTime > DateTime.MinValue)
                {
                    noCheckInlogRptDto.InTime = InTime;
                    report.Add(noCheckInlogRptDto);
                    InUseCheckOuts.Remove(server_handle);
                }
            }
        }

        public List<LogRptDto> GetReportRows()
        {
            return report;
        }

        private DateTime GetOutTime(int checkOutKey, LogFile logFile)
        {
            var checkout = logFile.CheckOuts[checkOutKey];
            string strCheckOutTime = $"{logFile.Year}/{checkout[CheckOut.mm_dd]} {checkout[CheckOut.time]}";
            return strCheckOutTime.GetFormattedDateTime();
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
    }
}