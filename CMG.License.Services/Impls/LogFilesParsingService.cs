using Akka.Actor;
using Akka.Streams;
using Akka.Streams.Dsl;
using CMG.License.Services.Interfaces;
using CMG.License.Shared.DataTypes;
using CMG.License.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CMG.License.Services.Impls
{
    public class LogFilesParsingService : ILogFilesParsingService
    {
        public LogFilesParsingService(IActorRefFactory actorSystem)
        {
            this.actorSystem = actorSystem;
        }
        private const string Pattern = "((?<=\")[^\"]*(?=\"(\\s|$)+)|(?<=\\s|^)[^\\s\"]*(?=\\s|$))";
        private readonly IActorRefFactory actorSystem;

        public async Task<LogFile> ParseLogFileEventsAsync(LogFile logFile)
        {
            if (!logFile.Exists())
                return null;
            var lines = File.ReadAllLines(logFile.Path);
            var startLine = lines.FirstOrDefault(l => l.StartsWith(LogEvents.START.ToString()));
            logFile.Start = ParseStart(startLine);

            var source = Source.From(lines);
           await source.RunForeach(l => ParseLine(logFile, l), actorSystem.Materializer());
            //var flow = Flow.Create<string, LogFile>();

            //foreach (var line in lines)
            //    ParseLine(logFile, line);
            return logFile;
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

        private bool ParseLine(LogFile logFile, string line)
        {
            var tokens = GetTokens(line);

            LogEvents tokenType;
            if (tokens.Any() && Enum.TryParse(tokens[0], out tokenType))
                switch (tokenType)
                {
                    case LogEvents.PRODUCT:
                        return logFile.Products.TryAdd(ParseProduct(tokens));
                    case LogEvents.IN:
                        return logFile.CheckIns.TryAdd(ParseCheckIn(tokens, logFile.Start.TimeStamp));
                    case LogEvents.OUT:
                        return logFile.CheckOuts.TryAdd(ParseCheckOut(tokens, logFile.Start.TimeStamp));
                    case LogEvents.DENY:
                        return logFile.Denys.TryAdd(ParseDeny(tokens, logFile.Start.TimeStamp));
                }
            return false;
        }

        private ProductDto ParseProduct(List<string> tokens) => new ProductDto
        {
            Name = tokens[Product.name],
            InstalledCount = Int32.Parse(tokens[Product.count])
        };

        private StartDto ParseStart(string startLine)
        {
            var tokens = GetTokens(startLine);
            var start = new StartDto();
            if (tokens.Any())
            {
                start.ServerName = tokens[Start.server_name];
                start.TimeStamp = $"{tokens[Start.date]} {tokens[Start.time]}"
                                 .GetFormattedDateTime("dd/MM/yyyy HH:mm:ss");
            }
            return start;
        }
    }
}