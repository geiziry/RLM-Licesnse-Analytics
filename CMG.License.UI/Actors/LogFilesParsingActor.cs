using Akka.Actor;
using CMG.License.Services.Interfaces;
using CMG.License.Shared.DataTypes;

namespace CMG.License.UI.Actors
{
    public class LogFilesParsingActor : ReceiveActor
    {
        private readonly ILogFilesParsingService logFilesParsingService;

        public LogFilesParsingActor(ILogFilesParsingService logFilesParsingService)
        {
            this.logFilesParsingService = logFilesParsingService;
            Receive<LogFile>(logFile => {
                logFilesParsingService.ParseLogFileEvents(ref logFile);
                Sender.Tell(logFile);
            });
        }
    }
}