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
        private readonly IActorRef getCheckInsActor;
        private readonly ILogFileRptGeneratorService logFileRptGeneratorService;
        private readonly ILogFilesParsingService logFilesParsingService;
        private readonly IDenialsRptGeneratorService denialsRptGeneratorService;

        public LogFileReportGeneratorActor(
            ILogFileRptGeneratorService logFileRptGeneratorService,
            ILogFilesParsingService logFilesParsingService,
            IDenialsRptGeneratorService denialsRptGeneratorService)
        {
            logFilesExcelProviderActor = Context.ActorOf(Context.DI().Props<LogFilesExcelProviderActor>(),
                                                                        ActorPaths.logFilesExcelProviderActor.Name);
            getCheckInsActor = Context.ActorOf(Context.DI().Props<GetCheckInsActor>(),
                                                                        ActorPaths.getCheckInsActor.Name);
            this.logFileRptGeneratorService = logFileRptGeneratorService;
            this.logFilesParsingService = logFilesParsingService;
            this.denialsRptGeneratorService = denialsRptGeneratorService;
            Receive<OpenLogFileViewModel>(viewModel => GenerateReportAsync(viewModel));
            Receive<string>(message => Context.Parent.Forward(message));
        }

        private void GenerateReportAsync(OpenLogFileViewModel viewModel)
        {
            logFileRptGeneratorService.InitializeReport();

            viewModel.OverallProgress = 0;
            viewModel.IsGeneratingReport = true;

            //var sourceQueue = Source.Queue<LogRptDto>(int.MaxValue, OverflowStrategy.Fail)
            //                                           .SelectAsyncUnordered(int.MaxValue, l => logFileRptGeneratorService.GetCheckInforInUseOuts(l, viewModel.LogFiles))
            //                                           .To(Sink.ActorRef<Tuple<bool, LogRptDto>>(logFilesExcelProviderActor, logFileRptGeneratorService.GetReportRows()))
            //                                           .Run(Context.Materializer());

            var g = RunnableGraph.FromGraph(GraphDsl.Create(b =>
            {
                var source = Source.From(viewModel.LogFiles);

                var sink = Sink.ActorRef<Tuple<bool, LogRptDto>>(logFilesExcelProviderActor, logFileRptGeneratorService.GetReportRows());

                var parsing = Flow.Create<LogFile>()
                                .Select(x =>
                                {
                                    viewModel.OverallProgress++;
                                    var parseTask = logFilesParsingService.ParseLogFileEventsAsync(x);
                                    parseTask.ContinueWith(t => denialsRptGeneratorService.Aggregate(t.Result));
                                    return parseTask.Result;
                                }).WatchTermination((_,o)=> { o.ContinueWith(t => getCheckInsActor.Tell(viewModel)); return _; });
                //.WatchTermination((_,u)=>u.PipeTo(getCheckInsActor));
                //TODO: create new actor to run getCheckIns                ===^
                //Akka.Done
                //Akka.Actor.Status.Failure;
                var reportGen = Flow.Create<LogFile>()
                                .SelectAsyncUnordered(int.MaxValue, logFileRptGeneratorService.GenerateReport)
                                .Recover(exception =>
                                {
                                    throw exception;
                                })
                                .SelectMany(x => x);
                //var getCheckIns = Flow.Create<LogRptDto>()
                //                        .SelectAsyncUnordered(int.MaxValue, x => getCheckInsActor.Tell(x))
                                            
                //var getCheckIns = Flow.Create<LogRptDto>()
                //                .SelectAsyncUnordered(int.MaxValue, l => logFileRptGeneratorService.GetCheckInforInUseOuts(l, viewModel.LogFiles));

                b.From(source).Via(parsing).Via(reportGen).To(Sink.ForEach<LogRptDto>(l=>getCheckInsActor.Tell(l)));//.Via(getCheckIns).To(sink);

                return ClosedShape.Instance;
            }));

            g.Run(Context.Materializer());
        }
    }
}