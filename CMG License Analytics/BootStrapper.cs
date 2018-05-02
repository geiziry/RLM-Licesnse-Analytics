using Autofac;
using CMG.License.Services.Impls;
using CMG.License.Services.Interfaces;
using CMG.License.UI;
using Prism.Autofac;
using Prism.Modularity;
using System.Windows;

namespace CMG.License.Analytics
{
    public class BootStrapper : AutofacBootstrapper
    {
        protected override void ConfigureContainerBuilder(ContainerBuilder builder)
        {
            builder.RegisterModule<UiModuleRegistery>();
            var LogFilesParsingService = new LogFilesParsingService();
            builder.RegisterInstance(LogFilesParsingService).As<ILogFilesParsingService>();
            base.ConfigureContainerBuilder(builder);
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