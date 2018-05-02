using Prism.Commands;
using Prism.Mvvm;

namespace CMG.License.UI.ViewModels
{
    public class OpenLogFileViewModel : BindableBase
    {
        private DelegateCommand getLogFilePathCmd;

        public DelegateCommand GetLogFilePathCmd
            => getLogFilePathCmd ?? (getLogFilePathCmd = new DelegateCommand(GetLogFilePath, () => true));

        private string logFilePath;

        public string LogFilePath
        {
            get { return logFilePath; }
            set => SetProperty(ref logFilePath, value);
        }

        private void GetLogFilePath()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            if (dialog.ShowDialog()==true)
            {
                LogFilePath = dialog.FileName;
            }
        }
    }
}