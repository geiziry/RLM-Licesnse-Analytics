using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
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
            Products = new List<ProductDto>();
            CheckIns = new List<CheckInDto>();
            CheckOuts = new List<CheckOutDto>();
            Denys = new List<DenyDto>();
        }

        public List<CheckInDto> CheckIns { get; set; }
        public List<CheckOutDto> CheckOuts { get; set; }
        public List<DenyDto> Denys { get; set; }
        public int Id { get; set; }
        public Dictionary<int, List<string>> InUses { get; set; }
        public DelegateCommand OpenFileCmd
                    => openFileCmd ?? (openFileCmd = new DelegateCommand(() => Process.Start("notepad.exe", Path),
                    () => !string.IsNullOrEmpty(Path)));

        public string Path { get { return filePath; } }
        public List<ProductDto> Products { get; set; }
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