using CMG.License.Services.Interfaces;
using CMG.License.Shared.DataTypes;
using CMG.License.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CMG.License.Services.Impls
{
    public class LogFilesParsingService : ILogFilesParsingService
    {
        private const string Pattern = "((?<=\")[^\"]*(?=\"(\\s|$)+)|(?<=\\s|^)[^\\s\"]*(?=\\s|$))";

        public LogFile ParseLogFileEvents(LogFile logFile)
        {
            if (!logFile.Exists())
                return null;
            var lines = File.ReadAllLines(logFile.Path);
            int i = 0;
            foreach (var line in lines)
            {
                var tokens = Regex.Matches(line, Pattern)
                .Cast<Match>()
                .Select(m => m.Value)
                .ToList();

                LogEvents tokenType;
                if (tokens.Any() && Enum.TryParse(tokens[0], out tokenType))
                    switch (tokenType)
                    {
                        case LogEvents.PRODUCT:
                            logFile.Products[i] = ParseProduct(tokens);
                            break;

                        case LogEvents.IN:
                            logFile.CheckIns[i] = ParseCheckIn(tokens);
                            break;

                        case LogEvents.OUT:
                            logFile.CheckOuts[i] = ParseCheckOut(tokens);
                            break;

                        case LogEvents.DENY:
                            logFile.Denys[i] = ParseDeny(tokens);
                            break;

                        case LogEvents.START:
                            logFile.Start = ParseStart(tokens);
                            break;

                        case LogEvents.INUSE:
                            logFile.InUses[i] = tokens;
                            break;

                        default:
                            break;
                    }
                i++;
            }
            return logFile;
        }

        private StartDto ParseStart(List<string> tokens) => new StartDto{
            ServerName = tokens[Start.server_name],
            TimeStamp = $"{tokens[Start.date]} {tokens[Start.time]}"
                                                    .GetFormattedDateTime("dd/MM/yyyy HH:mm:ss")
        };

        private DenyDto ParseDeny(List<string> tokens) => new DenyDto
        {
            Product=tokens[Deny.product],
            Version=tokens[Deny.version],
            Host=tokens[Deny.host],
            User=tokens[Deny.user],
            Count= Int32.Parse(tokens[Deny.count]),
            TimeStamp=$"{tokens[Deny.mm_dd]} {tokens[Deny.time]}"
                                                .GetFormattedDateTime("MM/dd HH:mm")
        };

        private CheckOutDto ParseCheckOut(List<string> tokens) => new CheckOutDto
        {
            Product=tokens[CheckOut.product],
            Version=tokens[CheckOut.version],
            Host=tokens[CheckOut.host],
            User=tokens[CheckOut.user],
            ServerHandle=tokens[CheckOut.server_handle],
            Count=Int32.Parse(tokens[CheckOut.count]),
            CurrentInUse=Int32.Parse(tokens[CheckOut.cur_use]),
            TimeStamp=$"{tokens[CheckOut.mm_dd]} {tokens[CheckOut.time]}"
                                                    .GetFormattedDateTime("MM/dd HH:mm:ss")
        };

        private CheckInDto ParseCheckIn(List<string> tokens) => new CheckInDto
        {
            Product=tokens[CheckIn.product],
            Version=tokens[CheckIn.version],
            Host=tokens[CheckIn.host],
            User=tokens[CheckIn.user],
            ServerHandle=tokens[CheckIn.server_handle],
            Count=Int32.Parse(tokens[CheckIn.count]),
            CurrentInUse=Int32.Parse(tokens[CheckIn.cur_use]),
            TimeStamp=$"{tokens[CheckIn.mm_dd]} {tokens[CheckIn.time]}"
                                                    .GetFormattedDateTime("MM/dd HH:mm:ss")
        };

        private ProductDto ParseProduct(List<string> tokens) => new ProductDto
        {
            Name=tokens[Product.name],
            InstalledCount=Int32.Parse(tokens[Product.count])
        };
    }
}