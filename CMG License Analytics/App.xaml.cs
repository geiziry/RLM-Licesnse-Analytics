using System.Windows;

namespace CMG.License.Analytics
{
    public partial class App : Application
    {
        BootStrapper bootStrapper;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            bootStrapper = new BootStrapper();
            bootStrapper.Run();
        }
    }
}