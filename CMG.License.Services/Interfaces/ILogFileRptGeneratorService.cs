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

        List<LogRptDto> GetReportRows();

        void InitializeReport();
    }
}