using Akka.Actor;
using Akka.DI.Core;
using Akka.Streams;
using Akka.Streams.Dsl;
using CMG.License.Services.Interfaces;
using CMG.License.Shared.AkkaHelpers;
using CMG.License.Shared.DataTypes;
using CMG.License.UI.ViewModels;
using System.Threading.Tasks;

namespace CMG.License.UI.Actors
{
    public class LogFileReportGeneratorActor : ReceiveActor
    {
        private readonly IActorRef logFilesExcelProviderActor;
        private readonly ILogFileRptGeneratorService logFileRptGeneratorService;
        private readonly ILogFilesParsingService logFilesParsingService;

        public LogFileReportGeneratorActor(
            ILogFileRptGeneratorService logFileRptGeneratorService,
            ILogFilesParsingService logFilesParsingService)
        {
            logFilesExcelProviderActor = Context.ActorOf(Context.DI().Props<LogFilesExcelProviderActor>(),
                                                                        ActorPaths.logFilesExcelProviderActor.Name);
            this.logFileRptGeneratorService = logFileRptGeneratorService;
            this.logFilesParsingService = logFilesParsingService;
            Receive<OpenLogFileViewModel>(async viewModel => await GenerateReportAsync(viewModel));
        }

        private async Task GenerateReportAsync(OpenLogFileViewModel viewModel)
        {
            logFileRptGeneratorService.InitializeReport();
            //initialize progress
            viewModel.OverallProgress = 0;
            viewModel.IsGeneratingReport = true;

            await Source.From(viewModel.LogFiles)
                 .SelectAsync(30, logFilesParsingService.ParseLogFileEventsAsync)
                 .RunWith(Sink.Ignore<LogFile>(), Context.Materializer());

            foreach (var logFile in viewModel.LogFiles)
            {
                logFileRptGeneratorService.GenerateReport(logFile);
                viewModel.OverallProgress++;
            }

            var reportRows = logFileRptGeneratorService.GetReportRows();
            logFilesExcelProviderActor.Tell(reportRows);
            viewModel.IsGeneratingReport = false;


            //await Source.From(viewModel.LogFiles)
            //    .SelectAsync(20, logFilesParsingService.ParseLogFileEventsAsync)
            //    .Async()
            //    .RunForeach(x =>
            //    {
            //        logFileRptGeneratorService.GenerateReport(x);
            //        viewModel.OverallProgress++;
            //    }, Context.Materializer())
            //    .ContinueWith((x) =>
            //    {
            //        var reportRows = logFileRptGeneratorService.GetReportRows();
            //        logFilesExcelProviderActor.Tell(reportRows);
            //        viewModel.IsGeneratingReport = false;
            //    });
        }
    }
}