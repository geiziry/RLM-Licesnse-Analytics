using Akka.Actor;
using Akka.DI.Core;
using CMG.License.Shared.AkkaHelpers;
using CMG.License.UI.ViewModels;

namespace CMG.License.UI.Actors
{
    public class OpenLogFileCoordinatorActor : ReceiveActor
    {
        public IActorRef ProgressTellerActor { get; }
        public IActorRef LogFileReportGeneratorActor { get; }

        public OpenLogFileCoordinatorActor(OpenLogFileViewModel viewModel)
        {
            ProgressTellerActor=Context.ActorOf(Props.Create(() => new ProgressTellerActor(viewModel)),
                                                    ActorPaths.ProgressTellerActor.Name);
            LogFileReportGeneratorActor= Context.ActorOf(Context.DI().Props<LogFileReportGeneratorActor>(),
                                                    ActorPaths.LogFileReportGeneratorActor.Name);
            Receive<string>(message => { if (message == "Start") LogFileReportGeneratorActor.Tell(viewModel); });
        }
    }
}