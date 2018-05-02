using System.Windows;
using Autofac;
using Prism.Autofac;

namespace CMG.License.Analytics
{
    public class BootStrapper:AutofacBootstrapper
    {
        protected override void ConfigureContainerBuilder(ContainerBuilder builder)
        {

            base.ConfigureContainerBuilder(builder);
        }

        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            Application.Current.MainWindow = (Window)Shell;
            Application.Current.MainWindow.Show();
        }


    }
}