using System.Windows;
using Autofac;
using CMG.License.UI;
using Prism.Autofac;
using Prism.Modularity;

namespace CMG.License.Analytics
{
    public class BootStrapper:AutofacBootstrapper
    {
        protected override void ConfigureContainerBuilder(ContainerBuilder builder)
        {

            builder.RegisterModule<UiModuleRegistery>();
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

        protected override void ConfigureModuleCatalog()
        {
            var uiModule = typeof(UI.UiModule);
            ModuleCatalog.AddModule(
                new ModuleInfo
                {
                    ModuleName = "uiModule",
                    ModuleType = uiModule.AssemblyQualifiedName,
                    InitializationMode = InitializationMode.WhenAvailable
                });
            base.ConfigureModuleCatalog();
        }
    }
}