using CMG.License.Services.Interfaces;
using CMG.License.Shared.DataTypes;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CMG.License.Services.Impls
{
    public class LogFilesParsingService : ILogFilesParsingService
    {
        public LogFile ParseLogFileEvents(LogFile logFile)
        {
            if (!logFile.Exists())
                return null;
            var lines = File.ReadAllLines(logFile.Path);
            int i = 0;
            foreach (var line in lines)
            {
                var tokens = Regex.Matches(line, "((?<=\")[^\"]*(?=\"(\\s|$)+)|(?<=\\s|^)[^\\s\"]*(?=\\s|$))") //@"[\""].+?[\""]|[^ ]+"
                .Cast<Match>()
                .Select(m => m.Value)
                .ToList();

                LogEvents tokenType;
                if (tokens.Any() && Enum.TryParse(tokens[0], out tokenType))
                    switch (tokenType)
                    {
                        case LogEvents.PRODUCT:
                            logFile.Products[i] = tokens;
                            break;

                        case LogEvents.IN:
                            logFile.CheckIns[i] = tokens;
                            break;

                        case LogEvents.OUT:
                            logFile.CheckOuts[i] = tokens;
                            break;

                        case LogEvents.DENY:
                            logFile.Denials[i] = tokens;
                            break;

                        case LogEvents.START:
                            logFile.Start = tokens;
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
    }
}