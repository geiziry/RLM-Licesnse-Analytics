using System.Collections.Generic;
using CMG.License.Shared.DataTypes;

namespace CMG.License.Services.Interfaces
{
    public interface ILogFileRptGeneratorService
    {
        List<LogRptDto> GenerateReport(LogFile logFile);
    }
}