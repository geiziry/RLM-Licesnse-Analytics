using Akka.Actor;
using CMG.License.Shared.AkkaHelpers;
using CMG.License.UI.ViewModels;

namespace CMG.License.UI.Actors
{
    public class OpenLogFileCoordinatorActor : ReceiveActor
    {
        private readonly OpenLogFileViewModel viewModel;

        public OpenLogFileCoordinatorActor(OpenLogFileViewModel viewModel)
        {
            this.viewModel = viewModel;
            ProgressTellerActor=Context.ActorOf(Props.Create(() => new ProgressTellerActor(viewModel)),
                                                    ActorPaths.ProgressTellerActor.Name);
        }

        private void GenerateReport()
        {
            logFileRptGeneratorService.InitializeReport();
            //initialize progress
            OverallProgress = 0;
            LogFiles.Clear();

            foreach (var logFileName in LogFileNames)
            {
                IsGeneratingReport = true;
                OverallProgress++;
                var logFile = new LogFile(logFileName);
                LogFiles.Add(logFile);
                logFilesParsingService.ParseLogFileEvents(ref logFile);
                logFileRptGeneratorService.GenerateReport(logFile);
            }

            var reportRows = logFileRptGeneratorService.GetReportRows();
            logFilesExcelProviderService.FillXlsxTemplate(reportRows, GetExcelTemplatePath(logFilePath));
        }

    }
}