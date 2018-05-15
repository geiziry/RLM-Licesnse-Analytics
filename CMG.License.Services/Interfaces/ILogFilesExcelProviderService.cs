using CMG.License.Shared.DataTypes;
using OfficeOpenXml;
using System.Collections.Generic;

namespace CMG.License.Services.Interfaces
{
    public interface ILogFilesExcelProviderService
    {
        void GenerateRawDataSheet(List<LogRptDto> ReportRows, ExcelPackage xlsFile);
        void GenerateDenialsSheet(List<LogRptDto> ReportRows, ExcelPackage xlsFile);
    }
}