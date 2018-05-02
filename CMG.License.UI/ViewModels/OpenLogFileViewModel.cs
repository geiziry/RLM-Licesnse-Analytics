using CMG.License.Services.Interfaces;
using CMG.License.Shared.DataTypes;
using CMG.License.Shared.Helpers;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.IO;

namespace CMG.License.UI.ViewModels
{
    public class OpenLogFileViewModel : BindableBase
    {
        private const string excelTemplate = "cmgusage log.xlsx";

        private readonly ILogFilesParsingService logFilesParsingService;

        private DelegateCommand generateReportCmd;

        private DelegateCommand getLogFilePathCmd;

        private List<string> logFileNames = new List<string>();

        private string logFilePath;

        public OpenLogFileViewModel(ILogFilesParsingService logFilesParsingService)
        {
            this.logFilesParsingService = logFilesParsingService;
        }

        public DelegateCommand GenerateReportCmd
            => generateReportCmd ?? (generateReportCmd = new DelegateCommand(GenerateReport, () => !string.IsNullOrEmpty(LogFilePath)));

        public DelegateCommand GetLogFilePathCmd
                            => getLogFilePathCmd ?? (getLogFilePathCmd = new DelegateCommand(GetLogFilePath, () => true));

        public string LogFilePath
        {
            get { return logFilePath; }
            set
            {
                SetProperty(ref logFilePath, value);
                GenerateReportCmd.RaiseCanExecuteChanged();
            }
        }

        private void GenerateReport()
        {
            if (!excelTemplate.ExtractResourceFile(logFilePath))
                return;
            foreach (var logFileName in logFileNames)
            {
                var logFile = new LogFile(logFileName);
                logFilesParsingService.ParseLogFileEvents(ref logFile);
            }
        }

        private void GetLogFilePath()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                logFileNames = new List<string>(dialog.FileNames);
                LogFilePath = Path.GetDirectoryName(dialog.FileNames[0]);
            }
        }
    }
}