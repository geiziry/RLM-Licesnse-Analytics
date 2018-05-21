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
        public LogFilesParsingService(IActorRefFactory actorSystem)
        {
            this.actorSystem = actorSystem;
        }

        private readonly IActorRefFactory actorSystem;

        public async Task<LogFile> ParseLogFileEventsAsync(LogFile logFile)
        {
            if (!logFile.Exists())
                return null;
            var lines = File.ReadAllLines(logFile.Path);
            var startLine = lines.FirstOrDefault(l => l.StartsWith(LogEvents.START.ToString()));
            logFile.ParseStart(startLine);
            logFile.InitializeProgress(lines);
            //var source = Source.From(lines);
            await Source.From(lines)
                .SelectAsyncUnordered(30, logFile.ParseLine)
                .RunWith(Sink.Ignore<bool>(), actorSystem.Materializer());
            //await source.RunForeach(l =>
            //{
            //    logFile.ParseLine(l);
            //    logFile.ProgressInt += (100d/ lines.Count());
            //}, actorSystem.Materializer());

            return logFile;
        }

    }
}