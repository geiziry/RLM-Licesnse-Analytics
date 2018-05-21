using Akka.Util;
using CMG.License.Shared.Helpers;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CMG.License.Shared.DataTypes
{
    public class LogFile : BindableBase
    {
        private const string Pattern = "((?<=\")[^\"]*(?=\"(\\s|$)+)|(?<=\\s|^)[^\\s\"]*(?=\\s|$))";

        private readonly string filePath;

        private DelegateCommand openFileCmd;
        private double progressInt;

        private double progressInc;

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
        public double ProgressInt
        {
            get { return progressInt; }
            set { SetProperty(ref progressInt, value); }
        }

        public string RawText { get; set; }
        public StartDto StartEvent { get; set; }

        public bool Exists()
        {
            return File.Exists(filePath);
        }

        private static List<string> GetTokens(string line)
        {
            var tokens = new List<string>();
            if (!string.IsNullOrEmpty(line))
                tokens = Regex.Matches(line, Pattern)
            .Cast<Match>()
            .Select(m => m.Value)
            .ToList();
            return tokens;
        }

        private CheckInDto ParseCheckIn(List<string> tokens, DateTime startTimeStamp) => new CheckInDto
        {
            Product = tokens[CheckIn.product],
            Version = tokens[CheckIn.version],
            Host = tokens[CheckIn.host],
            User = tokens[CheckIn.user],
            ServerHandle = tokens[CheckIn.server_handle],
            Count = Int32.Parse(tokens[CheckIn.count]),
            CurrentInUse = Int32.Parse(tokens[CheckIn.cur_use]),
            TimeStamp = $"{startTimeStamp.Year}/{tokens[CheckIn.mm_dd]} {tokens[CheckIn.time]}"
                                                    .GetFormattedDateTime("yyyy/MM/dd HH:mm:ss")
        };

        private CheckOutDto ParseCheckOut(List<string> tokens, DateTime startTimeStamp) => new CheckOutDto
        {
            Product = tokens[CheckOut.product],
            Version = tokens[CheckOut.version],
            Host = tokens[CheckOut.host],
            User = tokens[CheckOut.user],
            ServerHandle = tokens[CheckOut.server_handle],
            Count = Int32.Parse(tokens[CheckOut.count]),
            CurrentInUse = Int32.Parse(tokens[CheckOut.cur_use]),
            TimeStamp = $"{startTimeStamp.Year}/{tokens[CheckOut.mm_dd]} {tokens[CheckOut.time]}"
                                                            .GetFormattedDateTime("yyyy/MM/dd HH:mm:ss")
        };

        private DenyDto ParseDeny(List<string> tokens, DateTime startTimeStamp) => new DenyDto
        {
            Product = tokens[Deny.product],
            Version = tokens[Deny.version],
            Host = tokens[Deny.host],
            User = tokens[Deny.user],
            Count = Int32.Parse(tokens[Deny.count]),
            TimeStamp = $"{startTimeStamp.Year}/{tokens[Deny.mm_dd]} {tokens[Deny.time]}"
                                                .GetFormattedDateTime("yyyy/MM/dd HH:mm")
        };

        public Task<bool> ParseLine(string line)
        {
            var tokens = GetTokens(line);

            LogEvents tokenType;
            return Task.Run(() =>
            {
                ProgressInt += progressInc;
            if (tokens.Any() && Enum.TryParse(tokens[0], out tokenType))
                switch (tokenType)
                {
                    case LogEvents.PRODUCT:
                        return Products.TryAdd(ParseProduct(tokens));
                    case LogEvents.IN:
                            return CheckIns.TryAdd(ParseCheckIn(tokens, StartEvent.TimeStamp));
                    case LogEvents.OUT:
                            return CheckOuts.TryAdd(ParseCheckOut(tokens, StartEvent.TimeStamp));
                    case LogEvents.DENY:
                            return Denys.TryAdd(ParseDeny(tokens, StartEvent.TimeStamp));
                    default:
                        return false;
                }
                return false;
            });
        }

        private ProductDto ParseProduct(List<string> tokens) => new ProductDto
        {
            Name = tokens[Product.name],
            InstalledCount = Int32.Parse(tokens[Product.count])
        };

        public void ParseStart(string startLine)
        {
            var tokens = GetTokens(startLine);
            var start = new StartDto();
            if (tokens.Any())
            {
                start.ServerName = tokens[Start.server_name];
                start.TimeStamp = $"{tokens[Start.date]} {tokens[Start.time]}"
                                 .GetFormattedDateTime("MM/dd/yyyy HH:mm:ss");
            }
            StartEvent= start;
        }

        public void InitializeProgress(string[] lines)
        {
            ProgressInt = 0;
            progressInc = 100d / lines.Count();
        }

    }
}