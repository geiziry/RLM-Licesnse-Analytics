using Akka.Actor;
using CMG.License.Services.Interfaces;
using CMG.License.Shared.AkkaHelpers;
using CMG.License.Shared.DataTypes;
using CMG.License.UI.Actors;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace CMG.License.UI.ViewModels
{
    public class OpenLogFileViewModel : BindableBase
    {
        #region Properties

        private readonly ILogFileRptGeneratorService logFileRptGeneratorService;
        private readonly ILogFilesExcelProviderService logFilesExcelProviderService;
        private readonly ILogFilesParsingService logFilesParsingService;
        private DelegateCommand generateReportCmd;

        private DelegateCommand getLogFilePathCmd;

        private bool isGeneratingReport;
        private List<string> logFileNames = new List<string>();

        private string logFilePath;

        private int overallProgress = 0;

        public DelegateCommand GenerateReportCmd
            => generateReportCmd ?? (generateReportCmd = new DelegateCommand(GenerateReport, () => !string.IsNullOrEmpty(LogFilePath)));

        public DelegateCommand GetLogFilePathCmd
                            => getLogFilePathCmd ?? (getLogFilePathCmd = new DelegateCommand(GetLogFilePath, () => true));

        public bool IsGeneratingReport
        {
            get { return isGeneratingReport; }
            set { SetProperty(ref isGeneratingReport, value); }
        }

        public string LogFilePath
        {
            get { return logFilePath; }
            set
            {
                SetProperty(ref logFilePath, value);
                GenerateReportCmd.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<LogFile> LogFiles { get; set; }

        public int OverallProgress
        {
            get { return overallProgress; }
            set { SetProperty(ref overallProgress, value); }
        }

        public List<string> LogFileNames
        {
            get => logFileNames;
            set => SetProperty(ref logFileNames, value);
        }

        public IActorRef OpenLogFileCoordinatorActor { get; private set; }

        #endregion Properties

        public OpenLogFileViewModel(IActorRefFactory actorSystem, ILogFilesParsingService logFilesParsingService,
                    ILogFileRptGeneratorService logFileRptGeneratorService,
            ILogFilesExcelProviderService logFilesExcelProviderService)
        {
            this.logFilesParsingService = logFilesParsingService;
            this.logFileRptGeneratorService = logFileRptGeneratorService;
            this.logFilesExcelProviderService = logFilesExcelProviderService;
            LogFiles = new ObservableCollection<LogFile>();
            InitializeActors(actorSystem);
        }

        private void InitializeActors(IActorRefFactory actorSystem)
        {
            OpenLogFileCoordinatorActor = 
                actorSystem.ActorOf(Props.Create(() => new OpenLogFileCoordinatorActor(this)),
                                                    ActorPaths.OpenLogFileCoordinatorActor.Name);
        }

        private void GenerateReport()
        {
            logFileRptGeneratorService.InitializeReport();
            //initialize progress
            OverallProgress = 0;
            LogFiles.Clear();

            foreach (var logFileName in LogFileNames)
            {
                IsGeneratingReport = true;
                OverallProgress++;
                var logFile = new LogFile(logFileName);
                LogFiles.Add(logFile);
                logFilesParsingService.ParseLogFileEvents(ref logFile);
                logFileRptGeneratorService.GenerateReport(logFile);
            }

            var reportRows = logFileRptGeneratorService.GetReportRows();
            logFilesExcelProviderService.FillXlsxTemplate(reportRows, GetExcelTemplatePath(logFilePath));
        }

        //TODO: specify the generated Excel file name and path
        private string GetExcelTemplatePath(string logFilePath)
        {
            return Path.GetDirectoryName(logFilePath) + $@"\";
        }

        private void GetLogFilePath()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == true)
            {
                LogFileNames = new List<string>(dialog.FileNames);
                LogFilePath = Path.GetDirectoryName(dialog.FileNames[0]);
            }
        }
    }
}