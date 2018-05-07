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
            var logFilesParsingService = new LogFilesParsingService();
            builder.RegisterInstance(logFilesParsingService).As<ILogFilesParsingService>();
            var logFileRptGeneratorService = new LogFileRptGeneratorService();
            builder.RegisterInstance(logFileRptGeneratorService).As<ILogFileRptGeneratorService>();
            var logFilesExcelProviderService = new LogFilesExcelProviderService();
            builder.RegisterInstance(logFilesExcelProviderService).As<ILogFilesExcelProviderService>();

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