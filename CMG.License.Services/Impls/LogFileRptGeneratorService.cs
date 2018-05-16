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
            foreach (var checkOut in logFile.CheckOuts)
            {
                i++;
                logFile.ProgressInt = i * 100 / logFile.CheckOuts.Count;
                var logRptDto = new LogRptDto
                {
                    Product = checkOut.Product,
                    Version = checkOut.Version,
                    UserName = checkOut.User,
                    HostName = checkOut.Host,
                    InstalledCount = GetProductInstalledLicCount(checkOut.Product, logFile),
                    InUse = checkOut.CurrentInUse,
                    OutTime = checkOut.TimeStamp,
                    //InTime = CheckInTimeProcessingService.GetCheckInTime(checkOut, logFile),
                    //RequestTime = RequestTimeProcessingService.GetStrRequestTime(checkOut, logFile)
                };
                if (logRptDto.InTime > DateTime.MinValue)
                    report.Add(logRptDto);
                //else
                //    InUseCheckOuts[server_handle] = logRptDto;
            }
        }

        private void GetCheckInforInUseOuts(LogFile logFile)
        {
            foreach (var server_handle in InUseCheckOuts.Keys.ToList())
            {
                var noCheckInlogRptDto = InUseCheckOuts[server_handle];
                var InTime = CheckInTimeProcessingService.GetCheckInTime(noCheckInlogRptDto, server_handle, logFile);
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

        private int GetProductInstalledLicCount(string productName, LogFile logFile)
        {
            int licCount = 0;
            if (logFile.Products.Count > 0)
            {
                var product = logFile.Products.FirstOrDefault(x => x.Name == productName);
                    licCount = product.InstalledCount;
            }
            return licCount;
        }
    }
}