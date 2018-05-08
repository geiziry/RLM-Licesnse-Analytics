using CMG.License.Shared.DataTypes;
using System.Collections.Generic;

namespace CMG.License.Services.Interfaces
{
    public interface ILogFilesExcelProviderService
    {
        void FillXlsxTemplate(List<LogRptDto> ReportRows, string excelRptFilePath);
    }
}