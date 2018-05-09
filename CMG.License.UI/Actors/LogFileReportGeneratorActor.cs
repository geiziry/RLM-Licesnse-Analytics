using Akka.Actor;
using CMG.License.Services.Interfaces;
using CMG.License.Shared.DataTypes;
using CMG.License.UI.ViewModels;

namespace CMG.License.UI.Actors
{
    public class LogFileReportGeneratorActor : ReceiveActor
    {
        private readonly ILogFileRptGeneratorService logFileRptGeneratorService;
        private readonly ILogFilesParsingService logFilesParsingService;
        private readonly ILogFilesExcelProviderService logFilesExcelProviderService;

        public LogFileReportGeneratorActor(
            ILogFileRptGeneratorService logFileRptGeneratorService,
            ILogFilesParsingService logFilesParsingService,
            ILogFilesExcelProviderService logFilesExcelProviderService)
        {
            this.logFileRptGeneratorService = logFileRptGeneratorService;
            this.logFilesParsingService = logFilesParsingService;
            this.logFilesExcelProviderService = logFilesExcelProviderService;
            Receive<OpenLogFileViewModel>(viewModel =>  GenerateReport(viewModel));
        }

        private void GenerateReport(OpenLogFileViewModel viewModel)
        {
            logFileRptGeneratorService.InitializeReport();
            //initialize progress
            viewModel.OverallProgress = 0;
            viewModel.LogFiles.Clear();

            foreach (var logFileName in viewModel.LogFileNames)
            {
                viewModel.IsGeneratingReport = true;
                viewModel.OverallProgress++;
                var logFile = new LogFile(logFileName);
                viewModel.LogFiles.Add(logFile);
                logFilesParsingService.ParseLogFileEvents(ref logFile);
                logFileRptGeneratorService.GenerateReport(logFile);
            }

            var reportRows = logFileRptGeneratorService.GetReportRows();
            logFilesExcelProviderService.FillXlsxTemplate(reportRows, @"C:\Users\mgeiziry\Desktop\test.xlsx");
        }
    }
}