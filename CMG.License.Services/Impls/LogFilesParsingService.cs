using CMG.License.Services.Interfaces;
using CMG.License.Shared.DataTypes;
using System;
using System.IO;
using System.Linq;

namespace CMG.License.Services.Impls
{
    public class LogFilesParsingService : ILogFilesParsingService
    {
        public void ParseLogFileEvents(ref LogFile logFile)
        {
            if (!logFile.Exists())
                return;
            var lines = File.ReadAllLines(logFile.Path);
            int i = 0;
            foreach (var line in lines)
            {
                var tokens = line.Split(null).ToList();

                LogEvents tokenType;

                if (Enum.TryParse(tokens[0], out tokenType))
                    switch (tokenType)
                    {
                        case LogEvents.PRODUCT:
                            logFile.Products[i]=tokens;
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

                        default:
                            break;
                    }
                i++;
            }
        }
    }
}