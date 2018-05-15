using Akka.Actor;
using CMG.License.Services.Interfaces;
using CMG.License.Shared.DataTypes;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;

namespace CMG.License.UI.Actors
{
    public class LogFilesExcelProviderActor : ReceiveActor
    {
        public LogFilesExcelProviderActor(ILogFilesExcelProviderService logFilesExcelProviderService)
        {
            Receive<List<LogRptDto>>(reportRows =>
                    GenerateXlsFile(logFilesExcelProviderService, reportRows));
        }

        private static void GenerateXlsFile(ILogFilesExcelProviderService logFilesExcelProviderService, List<LogRptDto> reportRows)
        {
            const string excelRptFilePath = @"C:\Users\mgeiziry\Desktop\test.xlsx";
            //TODO: continue on formatting the pivot chart
            var xlsFileInfo = new FileInfo(excelRptFilePath);
            if (xlsFileInfo.Exists)
                xlsFileInfo.Delete();

            using (var xlsFile = new ExcelPackage(xlsFileInfo))
            {

                logFilesExcelProviderService.GenerateRawDataSheet(reportRows, xlsFile);

                xlsFile.Save();
            }
        }
    }
}