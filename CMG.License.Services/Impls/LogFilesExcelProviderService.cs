using CMG.License.Services.Interfaces;
using CMG.License.Shared.DataTypes;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CMG.License.Services.Impls
{
    public class LogFilesExcelProviderService : ILogFilesExcelProviderService
    {
        public void FillXlsxTemplate(List<LogRptDto> ReportRows, string excelTemplatePath)
        {
            var xlsFileInfo = new FileInfo(excelTemplatePath);
            if (!xlsFileInfo.Exists)
                return;

            using (var xlsFile=new ExcelPackage(xlsFileInfo))
            {
                var rowDataSheet = xlsFile.Workbook.Worksheets.Add("Raw Data2");
                var dataRange = rowDataSheet.Cells["A1"].LoadFromCollection(
                    from r in ReportRows
                    orderby r.OutTime
                    select r,
                    true, OfficeOpenXml.Table.TableStyles.Light13);
                xlsFile.Save();
            }
        }
    }
}