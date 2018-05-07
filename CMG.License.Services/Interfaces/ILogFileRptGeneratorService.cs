using CMG.License.Shared.DataTypes;
using System.Collections.Generic;

namespace CMG.License.Services.Interfaces
{
    public interface ILogFileRptGeneratorService
    {
        void GenerateReport(LogFile logFile);

        void InitializeReport();

        List<LogRptDto> GetReportRows();
    }
}