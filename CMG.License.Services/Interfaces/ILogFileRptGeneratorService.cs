using Akka.Util;
using CMG.License.Shared.DataTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMG.License.Services.Interfaces
{
    public interface ILogFileRptGeneratorService
    {
        Task<ConcurrentSet<LogRptDto>> GenerateReport(LogFile logFile);

        Task<Tuple<bool, LogRptDto>> GetCheckInforInUseOuts(LogRptDto logRptDto,IEnumerable<LogFile> logFiles);

        ConcurrentSet<LogRptDto> GetReportRows();

        void InitializeReport();
    }
}