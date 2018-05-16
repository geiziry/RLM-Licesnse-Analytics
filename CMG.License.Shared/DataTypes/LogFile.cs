using Akka.Util;
using Prism.Commands;
using Prism.Mvvm;
using System.Diagnostics;
using System.IO;

namespace CMG.License.Shared.DataTypes
{
    public class LogFile : BindableBase
    {
        private readonly string filePath;

        private DelegateCommand openFileCmd;
        private int progressInt;

        public LogFile(string filePath)
        {
            this.filePath = filePath;
            Products = new ConcurrentSet<ProductDto>();
            CheckIns = new ConcurrentSet<CheckInDto>();
            CheckOuts = new ConcurrentSet<CheckOutDto>();
            Denys = new ConcurrentSet<DenyDto>();
        }

        public ConcurrentSet<CheckInDto> CheckIns { get; set; }
        public ConcurrentSet<CheckOutDto> CheckOuts { get; set; }
        public ConcurrentSet<DenyDto> Denys { get; set; }
        public int Id { get; set; }
        public DelegateCommand OpenFileCmd
                    => openFileCmd ?? (openFileCmd = new DelegateCommand(() => Process.Start("notepad.exe", Path),
                    () => !string.IsNullOrEmpty(Path)));

        public string Path { get { return filePath; } }
        public ConcurrentSet<ProductDto> Products { get; set; }
        public int ProgressInt
        {
            get { return progressInt; }
            set { SetProperty(ref progressInt, value); }
        }

        public string RawText { get; set; }
        public StartDto Start { get; set; }

        public bool Exists()
        {
            return File.Exists(filePath);
        }
    }
}