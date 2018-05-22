using Akka.Actor;
using Akka.DI.Core;
using Akka.Streams;
using Akka.Streams.Dsl;
using Akka.Util;
using CMG.License.Services.Interfaces;
using CMG.License.Shared.AkkaHelpers;
using CMG.License.Shared.DataTypes;
using CMG.License.UI.ViewModels;
using System;
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
            var logRptDtoToCheck = new ConcurrentSet<LogRptDto>();

            //await Source.From(viewModel.LogFiles)
            //     .SelectAsyncUnordered(int.MaxValue, logFilesParsingService.ParseLogFileEventsAsync)//30
            //     .SelectAsyncUnordered(int.MaxValue, logFileRptGeneratorService.GenerateReport)//20
            //     .SelectMany(x => x)
            //     .RunWith(Sink.ForEachParallel<LogRptDto>(20,l=>logRptDtoToCheck.TryAdd(l)), Context.Materializer());

            //var t = new ConcurrentSet<bool>();
            //await Source.From(logRptDtoToCheck)
            //     .SelectAsyncUnordered(int.MaxValue, l => logFileRptGeneratorService.GetCheckInforInUseOuts(l, viewModel.LogFiles))
            //     .Async()
            //     .RunWith(Sink.ForEach<bool>(b=>t.TryAdd(b)), Context.Materializer());


            await Source.From(viewModel.LogFiles)
                 .SelectAsyncUnordered(int.MaxValue, logFilesParsingService.ParseLogFileEventsAsync)//30
                 .SelectAsyncUnordered(int.MaxValue, logFileRptGeneratorService.GenerateReport)//20
                 .SelectMany(x => x)
                 .SelectAsyncUnordered(int.MaxValue, l => logFileRptGeneratorService.GetCheckInforInUseOuts(l, viewModel.LogFiles))
                 .RunWith(Sink.Ignore<bool>(), Context.Materializer());


            //         viewModel.OverallProgress++;

            var reportRows = logFileRptGeneratorService.GetReportRows();
            logFilesExcelProviderActor.Tell(reportRows);
            viewModel.IsGeneratingReport = false;
        }
    }
}