using Akka.Actor;
using CMG.License.Shared.AkkaHelpers;
using CMG.License.Shared.DataTypes;
using CMG.License.UI.Actors;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace CMG.License.UI.ViewModels
{
    public class OpenLogFileViewModel : BindableBase
    {
        #region Properties

        private DelegateCommand generateReportCmd;

        private DelegateCommand getLogFilePathCmd;

        private bool isGeneratingReport=false;
        private List<string> logFileNames = new List<string>();

        private string logFilePath;

        private int overallProgress = 0;
        private ObservableCollection<LogFile> _logFiles;

        public DelegateCommand GenerateReportCmd
            => generateReportCmd ?? (generateReportCmd = new DelegateCommand(GenerateReport,
                () => !string.IsNullOrEmpty(LogFilePath) && !IsGeneratingReport));

        public DelegateCommand GetLogFilePathCmd
                            => getLogFilePathCmd ?? (getLogFilePathCmd = new DelegateCommand(GetLogFilePath, () => !IsGeneratingReport));

        public bool IsGeneratingReport
        {
            get { return isGeneratingReport; }
            set
            {
                SetProperty(ref isGeneratingReport, value);
                GenerateReportCmd.RaiseCanExecuteChanged();
                GetLogFilePathCmd.RaiseCanExecuteChanged();
            }
        }

        public List<string> LogFileNames
        {
            get => logFileNames;
            set => SetProperty(ref logFileNames, value);
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

        public ObservableCollection<LogFile> LogFiles
        {
            get => _logFiles;
            set => SetProperty(ref _logFiles, value);
        }

        public IActorRef OpenLogFileCoordinatorActor { get; private set; }

        public int OverallProgress
        {
            get { return overallProgress; }
            set { SetProperty(ref overallProgress, value); }
        }

        #endregion Properties

        public OpenLogFileViewModel(IActorRefFactory actorSystem)
        {
            InitializeActors(actorSystem);
        }

        private void GenerateReport()
        {
            OpenLogFileCoordinatorActor.Tell("Start");
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

                if (LogFileNames.Any())
                {
                    LogFiles = new ObservableCollection<LogFile>();
                    logFileNames.ForEach(x => LogFiles.Add(new LogFile(x)));
                    LogFilePath = Path.GetDirectoryName(dialog.FileNames[0]);
                }
            }
        }

        private void InitializeActors(IActorRefFactory actorSystem)
        {
            OpenLogFileCoordinatorActor =
                actorSystem.ActorOf(Props.Create(() => new OpenLogFileCoordinatorActor(this)),
                                                    ActorPaths.OpenLogFileCoordinatorActor.Name);
        }
    }
}