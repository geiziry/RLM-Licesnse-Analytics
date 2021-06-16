using Akka;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Streams;
using Akka.Streams.Dsl;
using CMG.License.Services.Interfaces;
using CMG.License.Shared.AkkaHelpers;
using CMG.License.Shared.DataTypes;
using CMG.License.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMG.License.UI.Actors
{
    public class GetCheckInsActor:ReceiveActor,IWithUnboundedStash
    {
        private readonly ILogFileRptGeneratorService logFileRptGeneratorService;
        private readonly IActorRef logFilesExcelProviderActor;
        private  OpenLogFileViewModel vm;

        public IStash Stash { get; set; }

        public GetCheckInsActor(ILogFileRptGeneratorService logFileRptGeneratorService)
        {
            logFilesExcelProviderActor = Context.ActorOf(Context.DI().Props<LogFilesExcelProviderActor>(),
                                                                        ActorPaths.logFilesExcelProviderActor.Name);
            var sourceQueue = Source.Queue<LogRptDto>(0, OverflowStrategy.Backpressure)
                .Recover(ex=>throw ex)
                .SelectAsyncUnordered(int.MaxValue, l => logFileRptGeneratorService.GetCheckInforInUseOuts(l, vm.LogFiles))
                .ToMaterialized(Sink.ActorRef<Tuple<bool, LogRptDto>>(logFilesExcelProviderActor, logFileRptGeneratorService.GetReportRows()),Keep.Left)
                .Run(Context.Materializer());

            Receive<OpenLogFileViewModel>(vmodel =>
            {
                vm = vmodel;
                Stash.UnstashAll();
                BecomeStacked(() =>
                {
                    Receive<LogRptDto>(l => sourceQueue.OfferAsync(l));
                });
            });

            ReceiveAny(_ => Stash.Stash());
            this.logFileRptGeneratorService = logFileRptGeneratorService;
        }


    }
}
