using Akka.Actor;
using CMG.License.UI.ViewModels;

namespace CMG.License.UI.Actors
{
    public class ProgressTellerActor : ReceiveActor
    {
        public ProgressTellerActor(OpenLogFileViewModel viewModel)
        {
            Receive<bool>(isGeneratingRpt => viewModel.IsGeneratingReport = true);
            Receive<int>(OverallProgress => viewModel.OverallProgress = OverallProgress);
        }
    }
}