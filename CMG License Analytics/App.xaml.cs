using Akka.Actor;
using System.Windows;

namespace CMG.License.Analytics
{
    public partial class App : Application
    {
        private static ActorSystem _appActorSystem=ActorSystem.Create("AppActorSystem");
        public static ActorSystem AppActorSystem => _appActorSystem;

        BootStrapper bootStrapper;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            bootStrapper = new BootStrapper();
            bootStrapper.Run();
        }
    }
}