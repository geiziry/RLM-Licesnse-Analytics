using Akka.Actor;
using Akka.Streams;
using Akka.Streams.Dsl;
using CMG.License.Services.Interfaces;
using CMG.License.Shared.DataTypes;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CMG.License.Services.Impls
{
    public class LogFilesParsingService : ILogFilesParsingService
    {
        private readonly IActorRefFactory actorSystem;

        public LogFilesParsingService(IActorRefFactory actorSystem)
        {
            this.actorSystem = actorSystem;
        }
        public async Task<LogFile> ParseLogFileEventsAsync(LogFile logFile)
        {
            if (!logFile.Exists())
                return null;
            var lines = File.ReadAllLines(logFile.Path);
            var startLine = lines.FirstOrDefault(l => 
                        l.StartsWith(LogEvents.START.ToString()));
            logFile.ParseStart(startLine);
            logFile.InitializeProgress(lines);
            await Source.From(lines)
                .SelectAsyncUnordered(30, logFile.ParseLine)
                .RunWith(Sink.Ignore<bool>(), actorSystem.Materializer());
            return logFile;
        }
    }
}