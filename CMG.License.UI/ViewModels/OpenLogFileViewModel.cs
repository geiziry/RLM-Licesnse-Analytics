using CMG.License.Services.Interfaces;
using CMG.License.Shared.DataTypes;
using CMG.License.Shared.Helpers;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace CMG.License.UI.ViewModels
{
    public class OpenLogFileViewModel : BindableBase
    {
        private const string excelTemplate = "cmgusage log.xlsx";

        private readonly ILogFileRptGeneratorService logFileRptGeneratorService;
        private readonly ILogFilesExcelProviderService logFilesExcelProviderService;
        private readonly ILogFilesParsingService logFilesParsingService;
        private DelegateCommand generateReportCmd;

        private DelegateCommand getLogFilePathCmd;

        private bool isGeneratingReport;
        private List<string> logFileNames = new List<string>();

        private string logFilePath;

        public OpenLogFileViewModel(ILogFilesParsingService logFilesParsingService,
            ILogFileRptGeneratorService logFileRptGeneratorService,
            ILogFilesExcelProviderService logFilesExcelProviderService)
        {
            this.logFilesParsingService = logFilesParsingService;
            this.logFileRptGeneratorService = logFileRptGeneratorService;
            this.logFilesExcelProviderService = logFilesExcelProviderService;
        }

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
        private void GenerateReport()
        {
            logFileRptGeneratorService.InitializeReport();

            foreach (var logFileName in logFileNames)
            {
                var logFile = new LogFile(logFileName);
                LogFiles.Add(logFile);
                logFilesParsingService.ParseLogFileEvents(ref logFile);
                logFileRptGeneratorService.GenerateReport(logFile);
            }

            var reportRows = logFileRptGeneratorService.GetReportRows();
            logFilesExcelProviderService.FillXlsxTemplate(reportRows, GetExcelTemplatePath(logFilePath));
        }

        private string GetExcelTemplatePath(string logFilePath)
        {
            return Path.GetDirectoryName(logFilePath) + $@"\{excelTemplate}";
        }

        private void GetLogFilePath()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == true)
            {
                logFileNames = new List<string>(dialog.FileNames);
                LogFilePath = Path.GetDirectoryName(dialog.FileNames[0]);
            }
        }
    }
}