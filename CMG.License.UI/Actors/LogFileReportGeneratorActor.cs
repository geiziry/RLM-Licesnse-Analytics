using Akka.Actor;
using Akka.DI.Core;
using Akka.Streams;
using Akka.Streams.Dsl;
using CMG.License.Services.Interfaces;
using CMG.License.Shared.AkkaHelpers;
using CMG.License.Shared.DataTypes;
using CMG.License.UI.ViewModels;

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
            Receive<OpenLogFileViewModel>(viewModel => GenerateReportAsync(viewModel));
        }

        private void GenerateReportAsync(OpenLogFileViewModel viewModel)
        {
            logFileRptGeneratorService.InitializeReport();
            //initialize progress
            viewModel.OverallProgress = 0;
            viewModel.IsGeneratingReport = true;

            Source.From(viewModel.LogFiles)
                .SelectAsync(4,logFilesParsingService.ParseLogFileEventsAsync)
                .Async()
                .RunForeach(x =>
                {
                    logFileRptGeneratorService.GenerateReport(x);
                    viewModel.OverallProgress++;
                }, Context.Materializer())
                .ContinueWith((x) =>
                {
                    var reportRows = logFileRptGeneratorService.GetReportRows();
                    logFilesExcelProviderActor.Tell(reportRows);
                    viewModel.IsGeneratingReport = false;
                });

            //Try Graph Dsl

            //var source = Source.From(viewModel.LogFiles);


            //var logFileParse = Flow.Create<LogFile>()
            //        .Select(logFile => logFilesParsingService.ParseLogFileEvents(logFile));
            //var graph = Flow.FromGraph(GraphDsl.Create(b =>
            //{
            //    var dispatchLogFile = b.Add(new Balance<LogFile>(2));

            //}));
            //var logFileGenerateRpt = Flow.Create<LogFile,Akka.NotUsed>().
            //    .Select(logFile =>
            //    {
            //        logFileRptGeneratorService.GenerateReport(logFile);
            //        viewModel.OverallProgress++;
            //    });

        }
    }
}