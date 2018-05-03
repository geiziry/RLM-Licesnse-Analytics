using CMG.License.Services.Interfaces;
using CMG.License.Shared.DataTypes;
using OfficeOpenXml;
using System.IO;

namespace CMG.License.Services.Impls
{
    public class LogFilesExcelProviderService : ILogFilesExcelProviderService
    {
        public void FillXlsxTemplate(LogFile logFile, string excelTemplate)
        {
            var xlsFileInfo = new FileInfo(Path.GetDirectoryName(logFile.Path) + $@"\{excelTemplate}");
            if (!xlsFileInfo.Exists)
                return;

            using (var xlsFile=new ExcelPackage(xlsFileInfo))
            {

            }
        }
    }
}