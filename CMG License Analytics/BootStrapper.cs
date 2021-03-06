﻿using Akka.Actor;
using Akka.DI.AutoFac;
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

            var logFilesParsingService = new LogFilesParsingService(App.AppActorSystem);
            builder.RegisterInstance(logFilesParsingService).As<ILogFilesParsingService>();

            var logFileRptGeneratorService = new LogFileRptGeneratorService(App.AppActorSystem);
            builder.RegisterInstance(logFileRptGeneratorService).As<ILogFileRptGeneratorService>();

            var logFilesExcelProviderService = new LogFilesExcelProviderService();
            builder.RegisterInstance(logFilesExcelProviderService).As<ILogFilesExcelProviderService>();

            var denialsRptGeneratorService = new DenialsRptGeneratorService();
            builder.RegisterInstance(denialsRptGeneratorService).As<IDenialsRptGeneratorService>();

            builder.RegisterInstance(App.AppActorSystem).As<IActorRefFactory>().ExternallyOwned();
            base.ConfigureContainerBuilder(builder);
        }

        protected override void ConfigureModuleCatalog()
        {
            var uiModule = typeof(UiModule);
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
            var reslover = new AutoFacDependencyResolver(Container, App.AppActorSystem);
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