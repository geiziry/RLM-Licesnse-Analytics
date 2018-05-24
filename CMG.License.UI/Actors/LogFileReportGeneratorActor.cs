using Akka.Actor;
using Akka.DI.Core;
using Akka.Streams;
using Akka.Streams.Dsl;
using CMG.License.Services.Interfaces;
using CMG.License.Shared.AkkaHelpers;
using CMG.License.Shared.DataTypes;
using CMG.License.UI.ViewModels;
using System;

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
            Receive<string>(message => Context.Parent.Forward(message));
        }

        private void GenerateReportAsync(OpenLogFileViewModel viewModel)
        {
            logFileRptGeneratorService.InitializeReport();

            viewModel.OverallProgress = 0;
            viewModel.IsGeneratingReport = true;

            var g = RunnableGraph.FromGraph(GraphDsl.Create(b =>
            {
                var source = Source.From(viewModel.LogFiles);

                var sink = Sink.ActorRef<Tuple<bool, LogRptDto>>(logFilesExcelProviderActor, logFileRptGeneratorService.GetReportRows());

                var parsing = Flow.Create<LogFile>()
                                .Select(x =>
                                {
                                    viewModel.OverallProgress++;
                                    return logFilesParsingService.ParseLogFileEventsAsync(x).Result;
                                });

                var reportGen = Flow.Create<LogFile>()
                                .SelectAsyncUnordered(int.MaxValue, logFileRptGeneratorService.GenerateReport)
                                .SelectMany(x => x);

                var getCheckIns = Flow.Create<LogRptDto>()
                                .SelectAsyncUnordered(int.MaxValue, l => logFileRptGeneratorService.GetCheckInforInUseOuts(l, viewModel.LogFiles));

                b.From(source).Via(parsing).Via(reportGen).Via(getCheckIns).To(sink);

                return ClosedShape.Instance;
            }));

            g.Run(Context.Materializer());
        }
    }
}