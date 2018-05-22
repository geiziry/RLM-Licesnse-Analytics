using Akka.Util;
using CMG.License.Shared.DataTypes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMG.License.Services.Interfaces
{
    public interface ILogFileRptGeneratorService
    {
        Task<ConcurrentSet<LogRptDto>> GenerateReport(LogFile logFile);

        Task<bool> GetCheckInforInUseOuts(LogRptDto logRptDto,IEnumerable<LogFile> logFiles);

        List<LogRptDto> GetReportRows();

        void InitializeReport();
    }
}