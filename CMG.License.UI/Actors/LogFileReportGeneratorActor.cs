using Akka.Actor;
using Akka.DI.Core;
using CMG.License.Services.Interfaces;
using CMG.License.Shared.AkkaHelpers;
using CMG.License.UI.ViewModels;
using System;
using System.Threading.Tasks;

namespace CMG.License.UI.Actors
{
    public class LogFileReportGeneratorActor : ReceiveActor
    {
        private readonly IActorRef LogFilesParsingActor;
        private readonly IActorRef logFilesExcelProviderActor;
        private readonly ILogFileRptGeneratorService logFileRptGeneratorService;

        public LogFileReportGeneratorActor(
            ILogFileRptGeneratorService logFileRptGeneratorService)
        {
            LogFilesParsingActor = Context.ActorOf(Context.DI().Props<LogFilesParsingActor>(),
                                                                        ActorPaths.LogFilesParsingActor.Name);
            logFilesExcelProviderActor= Context.ActorOf(Context.DI().Props<logFilesExcelProviderActor>(),
                                                                        ActorPaths.logFilesExcelProviderActor.Name);
            this.logFileRptGeneratorService = logFileRptGeneratorService;

            Receive<OpenLogFileViewModel>(async viewModel => await GenerateReportAsync(viewModel));
        }

        private async Task GenerateReportAsync(OpenLogFileViewModel viewModel)
        {
            logFileRptGeneratorService.InitializeReport();
            //initialize progress
            viewModel.OverallProgress = 0;

            foreach (var logFile in viewModel.LogFiles)
            {
                viewModel.IsGeneratingReport = true;
                viewModel.OverallProgress++;
                await LogFilesParsingActor.Ask(logFile, TimeSpan.FromSeconds(2));
                logFileRptGeneratorService.GenerateReport(logFile);
            }

            var reportRows = logFileRptGeneratorService.GetReportRows();
            logFilesExcelProviderActor.Tell(reportRows);
            viewModel.IsGeneratingReport = false;
        }

    }
}